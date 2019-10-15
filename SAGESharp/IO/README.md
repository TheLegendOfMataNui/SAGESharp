# Serialization in SAGESharp
----------------------------

## Binary Serialization

The [binary file formats used by *SAGE*](https://github.com/TheLegendOfMataNui/sage-file-formats) are not standard and require custom code in order to read and write them.

In order to read and write *SAGESharp* provides the following interfaces and classes under the `SAGESharp.IO` namespace. Client code should only call these directly.

- `IBinaryReader` to read primitive types from a stream (ex: read a `short`).
  - To create a new instance use the `Reader.ForStream(Stream)` static method.
- `IBinaryWriter` to write primitive types to a streams (ex: write a `uint`).
  - To create a new instance use the `Writer.ForStream(Stream)` static method.
- `IBinarySerializer<T>` to read/write an object using the interfaces mentioned above:
  - To create a new binary serializer for an [SLB format](https://github.com/TheLegendOfMataNui/sage-file-formats/tree/master/SLB) use the `BinarySerializer.ForType<T>()` static method.
  - To create a new binary serializer for the [BKD format](https://github.com/TheLegendOfMataNui/sage-file-formats/tree/master/BKD) use the `BinarySerializer.ForBKDFiles` static property.
- `IBinarySerializable` to represent an object that can read or write itself by using `IBinaryReader` and `IBinaryWriter`.

All other classes related to binary serialization in the `SAGESharp.IO` namespaces are implementation details and should not be used outside the namespace unless explictly said otherwise.

### SLB Serialization

#### Format

[SLB files](https://github.com/TheLegendOfMataNui/sage-file-formats/tree/master/SLB) follow a very particular format: all files contain data structures that can be expressed as classes and the layout of the class' values in binary almost matches the binary writen to the files, there are some quirks though:

1. If a file contains a list, instead of writing the entry count and followed by the entries of the list the list count and an *offset* with the contents of the list is written, usually these elements can be found after all the instances of the current class, ex: if a class `A` contains a list of class `B` and a list of class `C` the file would contain first all the values for class `A` then all the values for the list of type `B` and last all the values for the list of type `C`.
2. Sometimes the entry count for a list is duplicated in the file.
3. Strings can appear as an *offset* (just as described for the lists) but the length of the string is contained in a byte at the offset location and is null terminal.
4. Strings can appear *inline* but the length of these strings is predefined.
5. Some values will have a *padding* of a couple of bytes after the value itself.
6. SLB files contain a footer at the end of the file with a list of *offsets* where the other *offsets* (for strings and list) are located, this footer must be *aligned*.

All of these quirks (except by the footer) are expressed with the properties defined at the *Attributes.cs* file.

#### How to use it

Usage is very simple, just ensure the class representing your SLB file is correctly annotated (as described in the format section):

```C#
IBinarySerializer<T> serializer = BinarySerializer.ForType<MySLBType>();
using (Stream stream = FileStream(getFileName(), System.IO.FileMode.Open))
{
    IBinaryReader reader = Reader.ForStream(stream);
    MySLBType mySLB = serializer.Read(reader);
}
```

#### Implementation

**These are details on how the serialization logic is implemented and client code shouldn't depend on such details.**

Given all the quirks in SLB files the way serialization works the solution is to describe an SLB file format with a tree, then this tree can be used to read or write an instance of the class for that SLB format. To fit with the weirdness of SLB formats these trees are not the *normal* trees used in programming excersises where all the nodes share a common type and store the same kind of data, there are different types of nodes to describe different types of values:

- `IDataNode` expresses a value that is *inline* in the file.
  - These nodes have a list of `IEdge` that them to other nodes that can be of any type.
  - All trees have `IDataNode` as the root node.
- `IOffsetNode` expresses a value that will appear at a later position (an *offset*) in the file.
  - These nodes have a child `IDataNode` that expresses how to write the actual value.
- `IListNode` expresses a list of values, the number of values is *inline* in the file but the values themselves appear at a later position (an *offset*).
  - `IListNode` inherits from `IOffsetNode` as the offset behaviour is the same
  - These nodes have a child `IDataNode` that expresses how to write the actual value of the entries.

In order to build a tree the `TreeBuilder.BuildTreeForType(Type)` static method is used.

Given a file and a tree that represents such file the `TreeReader.Read(IBinaryReader, IDataNode)` method can read the file and transform it into an instance of a class. The same is true for writing with the `TreeWriter.Write(IBinaryWriter, IDataNode, object)` method where the last argument is the value to write. The later will return a list of *offsets* to generate the footer.

The tree builder, writer and reader are used by the `TreeBinarySerializer<T>` class which implements `IBinarySerializer<T>` to serialize and deserialize SLB objects.

### BKD Serialization

The [BKD format](https://github.com/TheLegendOfMataNui/sage-file-formats/tree/master/BKD) is not very complicated but doesn't follow the same rules as SLB formats, so the `BKD` class implements `IBinarySerializable` so it can be serialized with the `BinarySerializer.ForBKDFiles` serializer:

```C#
BKD bkd = null;
using (Stream stream = FileStream(getFileName(), System.IO.FileMode.Open))
{
    IBinaryReader reader = Reader.ForStream(stream);
    bkd = BinarySerializer.ForBKDFiles.Read(reader);
}
```
