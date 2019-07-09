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

Globals
-------
Many developers believe that global state is distasteful and poor design, but sometimes it makes game development, which mainly focuses on accomplishing tasks by deadlines and not theoretical elegance, a bit more straightforward and manageable.  
To declare a global variable, use the `global` keyword outside any function or class, like so:
```
global globalName;
```
Note that LSS does not accomodate initialization of globals within the declaration syntax. Put that inside a subroutine somewhere.

Functions
---------
Functions are global subroutines without an associated class.
```
function MyFunction(parameter1, parameter2) {
    ...
}
```

Subroutine Statements
-----------
Subroutines (both class methods and standalone functions) are composed of a name, parameters, and a block of statements.
Each statement in the block can be one of a few things:

**Assignment**  
```
target = value;
```
Assignments set the value of an assignable variable to the value specificed by the given expression.  
`target`: An expression that is assignable. These include member values, variables, game variables, and array elements.  
`value`: An expression to evaluate that will be assigned to the target.

**Block**  
```
{ statement1, statement2, [...] }
```
Block statements execute a sequence of zero or more contained statements in order.

**Expression**  
```
DoThing() + 1;
```
Expression statements evaluate the expression and discard the result. They are usually used to call subroutines.

**If**  
```
if (condition)
    statement
else if (condition2)
    statement2
[...]
else
    statement3
```
If statements execute their attached statement if the condition expression evaluates to `true` or an integer value greater than `0`.  
Else branches are only tested if the condition above them was tested and the corresponding statement was not executed.  
`condition`, `condition2`, `condition3`: The expressions that will be evaluated when determining whether to execute the associated statements.  
`statement`, `statement2`, `statement3`: The statements to be executed if the corresponding condition of the if statement is evaluated and found to be `true` or an integer greater than `0`.  

**For Each**
```
foreach (var thing : collection)
    statement
```
For Each statements execute their attached statement one time for each element in the given collection, with the declared variable taking on the value of the element.  
Note that unlike many other languages, the collection expression is evaluated once to find the length, and then again each time the value variable is evaluated.  
Also unlike many languages, the `var` keyword must be used - you can't re-use an existing variable as the iteration variable.

**While**
```
while (condition)
    statement
```
While statements repeatedly evaluate the condition, and if the condition evaluates to `true` or an integer greater than `0`, execute the associated statement, until the condition does not evaluate to `true` or an integer greater than `0`.  
`condition`: The condition to be evaluated when determining whether the statement shall be executed.  
`statement`: The statement that is executed when the condition is evaluated to either `true` or an integer greater than `0`.  

**Return**
```
return value;
```
Exits the subroutine and returns the value of the given expression to the subroutine which called the returning subroutine.  
If `value` is not supplied, it is assumed to be `null`.
`value`: The value to be sent back to the subroutine which called the returning subroutine.

Values
------
While variables do not have a type, the values that they hold do have types.

**Instance** (reference type)
Instances are objects that have been created from a script `class` using the `new` keyword.

**Game Object** (reference type)
Game Objects are instances of native C++ data structures. Script cannot directly access their members, but instead passes them to native game functions to manipulate them.

**Array** (reference type)
Arrays are non-homogenous, resizable lists of values.

**null** (immutable value type)
Null values represent the idea of 'there is no value here'.

**Integer** (immutable value type)
Integer values are 32-bit signed whole numbers.

**String** (immutable value type)
String values are lists of characters that make up text.

**Float** (immutable value type)
Float values are 32-bit signed floating-point numbers. They can hold fractional parts, but their precision decreases as the magnitude of the number increases.

**Color** (immutable value type)
Color values are 32 bits each (8 bits for each of the 4 channels), and each of the Red, Green, Blue, and Alpha channels are stored as a unsigned byte (0-255).  
  
Note that because Color values are immutable, you cannot change an individual channel of an existing color.  
Instead, use the `.__withred(value)` and similar builtin methods to return modified copies of existing color values.

Expressions
-----------
Expressions are bits of code that evaluate to a value, which can be of many different types.  
Here are the various types of expressions:  

**Literals**  
LSS supports string, integer, and floating-point literals.  
Colors are an interesting case - they are primitives, but the literal looks like a function call and can contain arbitrary expressions for the red, green, blue, and alpha arguments.  
```
"this is a string"
10000
-4193.9591
rgba(255, 0, 255 / 2, myVariable3)
```

**Variables**  
Global variables, parameters, and local variables (declared in the subroutine) can all be accessed simply by their name.
```
myGlobal1
theCoolerParam1
theFontSizeToSetRightNow
```

**Subroutine Calls**  
Functions and member methods are called by their name followed by an open parenthesis, the arguments to pass to the subroutine, and then a closing parenthesis.  
Member methods must be called on an instance of the class that contains the method.
```
functionThatDoesSomething("5", 10.5)
myRectangle.someMethodToGetTheArea(100)
```

**Game Variables and Function Calls**  
The native host may expose variables and functions to the LSS script. They are identified with a namespace and a name.  
*NOTE: Game variables are not functional in The Legend of Mata Nui.*
```
namespace::variableName
someOtherNamespace::functionName(argument1, argument2)
```

**Member Variables**  
To access a member (for example, a property) of an object, use a period followed by the member name.
```
mySquare.width
```

**Runtime Binding**  
Sometimes, there is no way to know at compile time which method should be called or which property should be accessed.  
So, LSS provides the dollar sign `$` to look up members and functions given an expression that will be evaluated to determine which member to access each time the expression is evaluated.
This applies to member variables and methods (`.$`) and game functions (but not game variables) (`::$`).  
The expression after the dollar sign must be a literal, a variable name, or a parenthesized expression.
```
myRectangle.$("wid" + "th") // Variable access
someNamespace::$variableContainingFunctionName(arg1, 1052, "hey there") // Game function call
myRectangle.$(this.funcName)(arg1, arg2) // this.funcName is not a single variable name, and thus must be parenthesized
```

**Instantiation**  
To create a new instance of an LSS class or native structure, the `new` keyword is used to invoke the constructor.
```
new lego_pickup("mskv", 3, 1, "item", 1, 1)
```

**Logical Operators**  
LSS contains the standard operators you are probably used to for making comparisons.  
One difference is that the `&&` and `||` operators do not short-circuit.
```
expression1 == expression2 // Checks equality
expression1 != expression2 // Checks inequality
expression1 < expression2 // Checks less than
expression1 <= expression2 // Checks less than or equal to
expression1 > expression2 // Checks greater than
expression1 >= expression2 // Checks greater than or equal to
!expression1 // The logical opposite
expression1 && expression2 // Checks whether both are true.
expression1 || expression2 // Checks whether at least one of the expressions are true.
```

**Mathematic Operators**  
LSS also contains operators for math operations, with a few deviations from the standard C operators, notably:  
 - Caret (`^`) is the power operator
 - No increment or decrement operators
```
-thing // Negate
thing ^ thing // Exponent
thing * thing // Product
thing / thing // Quocient
thing % thing // Modulo
thing + thing // Sum
thing - thing // Difference
```

**Bitwise Operators**  
LSS also contains operators for bitwise integer operations, with a few deviations as well from standard C operators:  
 - Octothorpe (`#`) is the bitwise XOR operator
```
~thing // Bitwise NOT
thing << thing // Bitwise left shift
thing >> thing // Bitwise right shift
thing & thing // Bitwise AND
thing # thing // Bitwise XOR
thing | thing // Bitwise OR
```

**Arrays**    
LSS provides the standard array access syntax and a succinct syntax for creating array expressions, both using square brackets.
```
[ ]
[ thing1, thing2, 1003919, "Sup guys", true ]
arrayThing[0] = 10
arrayThing[12 + 3] = arrayThing[0]
```

**Builtin Members**  
Some names, which start with two underscores, are reserved for special use.
These members only succeed on array or color values, and so are not permitted on `this`, which can never be an array or color value.
```
thing.__length // Gets array length
thing.__red // Get Red component of color primitive
thing.__green // Get Green component of color primitive
thing.__blue // Get Blue component of color primitive
thing.__alpha // Get Alpha component of color primitive
thing.__withred(value) // Gets a copy of the color in 'thing', with the Red channel set to the given value
thing.__withgreen(value) // Gets a copy of the color in 'thing', with the Green channel set to the given value
thing.__withblue(value) // Gets a copy of the color in 'thing', with the Blue channel set to the given value
thing.__withalpha(value) // Gets a copy of the color in 'thing', with the Alpha channel set to the given value
```

**Builtin Functions**  
LSS reserves some keywords for builtin functions, which start with two underscores, for type manipulation.
Unlike builtin members, these may be called on `this`.
```
thing.__append(elementToAppend) // Appends to array, returns the input array, which has been mutated.
thing.__removeat(index) // Removes the array element at the given index, returns the input array, which has been mutated.
thing.__insertat(index, elementToInsert) // Inserts the given element at the given index, returns the input array, which has been mutated.
__tostring(expression)
__tofloat(expression)
__toint(expression)
__isint(expression)
__isfloat(expression)
__isstring(expression)
__isobject(expression)
__isinstance(expression)
__isarray(expression)
__classid(expression)
```