# aktos-dcs-cs

This is the C# port of [aktos-dcs](https://github.com/ceremcem/aktos-dcs) platform. 

# Testing 

1. Download and install [aktos-dcs](https://github.com/ceremcem/aktos-dcs)
* Between C# and Python applications: 
  2. Run `aktos-dcs/examples/ponger.py`
  3. Run `aktos-dcs-cs/examples/pinger` solution
  4. See if they are pinging and ponging each other. 
* Between C# applications: 
   1. Run `aktos-dcs/examples/foo.py` (in order to create a router) 
   2. Run `examples/ponger` solution. 
   3. Run `examples/pinger` solution 
   4. See they are messaging.

# Usage

1. Create a new project 
2. Add `aktos-dcs-cs` references in your project. 
   1. In "Solution Explorer", click `Add -> Solution`, browse to select `aktos-dcs-cs/aktos-dcs-cs/aktos-dcs-cs.csproj`
   2. In "Solution Explorer", in your new project, click "add references", navigate to "projects/solution" on the left bar, add "aktos_dcs_cs"
3. In your project, add `using aktos_dcs_cs;` line at the top of the file. 

4. Create and use classes that are using `Actor`s. (see below)
5. Prevent your program from ending. 
   
      > Add something that will block your main loop's execution (like "Press a key to continue" line) or if your application is a GUI application, it probably has its own main loop, which will suffice. You may also simply add `Actor.wait_all()` at the end of program, which is a [forever sleeping loop under the hood](https://github.com/ceremcem/aktos-dcs-cs/blob/master/aktos-dcs-cs/actor.cs#L247-L253). 

6. See your objects are messaging with each other. 

# Creating and using `Actor` based classes
There are 2 way of usage `aktos_dcs_cs.Actor` in your application. 
 
1. Create classes that are inherited from `Actor` and do any of your application work in these classes. 
2. Create an `Actor` based class and use it in another class as a communicator object. 


### 1. If you inherited your application classes from `Actor`: 
1. Create any number of classes which are inherited from `Actor`
2. Do your blocker works in `public override action(){ ... }` method
3. Send any message to others via `send(object)` method
4. Receive others' messages via `public override receive(message){ ... }` method
5. Receive others' messages via `public handle_SUBJECT(message){...}` methods if you know the subject. 
6. Initialize your objects from these `Actor` based classes

### 2. If you need to use communicator objects in another class 
1. Create a class (`MyCommunicator`) based on `Actor` which will serve your `another class` as a communicator object (like in [the example](https://github.com/ceremcem/aktos-dcs-cs/blob/master/examples/gui-example/gui-example/Form1.cs#L42-L54)) 
2. In your communicator class, define events as ` public event msg_callback event_SUBJECT;`
3. Any message which has the `SUBJECT` subject will trigger this event. 
4. Define a handler in your `another class` that will handle messages about `SUBJECT`, preferably define them as 
      
        private void handle_SUBJECT(Dictionary<string, object> msg)
        {
            // use "msg" now as incoming message 
        }

5. Create a `communicator_object` and add your handler method to the event handler in your `another class`' constructor, like: 

          MyCommunicator comm = new MyCommunicator(); 
          comm.event_SUBJECT += handle_SUBJECT; 
          

**Note: DO_NOT perform blocker operations in receiver methods.**
