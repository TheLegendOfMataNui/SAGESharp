Litestone Script
================

Litestone Script (LSS) is a high-level language for writing code for OSI virtual machines.

The syntax is designed to feel as C-style as possible, while dropping static typing and incorporating OSI-specific functionality.

Scripts can contain classes and/or functions, and classes and functions may only be defined and declared at the script scope.

Comments
--------
Before we get into actually doing anything, you ought to know that comments are just standard C syntax (`//` and `/* */`).
You probably guessed this, and you were right.

Classes
-------
Like other object-oriented programming languages, classes define new data types.
They can contain member subroutines (methods) and instance variables (properties).  
Constructors are just methods named the same as their containing class.
Because of this freedom, you may or may not call arbitrary constructors (like any other method) from anywhere in any method.
That doesn't mean it's a good idea though. As they say, with great power...
```
class MyClass : ParentClass {
    property MyThing;
    property MyOtherThing;

    // Constructor recieving no arguments besides the implicit 'this'
    method MyClass() {
        ...
    }

    // Implicitly recieves 'this' in addition to 'parameter1'
    method MyInstanceMethod(parameter1) {
        ...
    }
}
```

Functions
---------
Functions are global subroutines without an associated class.
```
function MyFunction(parameter1, parameter2) {
    ...
}
```

Subroutines
-----------
Subroutines (both methods and functions) are composed of a name, parameters, and a block of statements.
Each statement in the block can be one of a few things: