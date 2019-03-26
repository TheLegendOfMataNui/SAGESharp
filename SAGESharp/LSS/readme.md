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

Expressions
-----------
Expressions are bits of code that evaluate to a value, which can be of many different types.  
Here are the various types of expressions:  

**Literals**  
LSS supports string, integer, and floating-point literals, which look like:
```
"this is a string"
10000
-4193.9591
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
myRectangle.$("wid" + "th")
someNamespace::$variableContainingFunctionName(arg1, 1052, "hey there")
```