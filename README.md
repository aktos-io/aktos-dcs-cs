# aktos-dcs-cs

This is the C# port of [aktos-dcs](https://github.com/ceremcem/aktos-dcs) platform. 

# Testing 

1. Between C# applications: 
   1. run `examples/ponger` solution. 
   2. run `examples/pinger` solution 
   3. see they are messaging.
2. Between C# and Python applications: 
  1. Download and install [aktos-dcs](https://github.com/ceremcem/aktos-dcs)
  2. Run `aktos-dcs/examples/ponger.py`
  3. Run `aktos-dcs-cs/examples/pinger` solution
  4. See if they are pinging and ponging each other. 

# Usage

1. Create a new project 
2. In "Solution Explorer", click `Add -> Solution`, browse to select `aktos-dcs-cs/aktos-dcs-cs/aktos-dcs-cs.csproj`
3. In "Solution Explorer", in your new project, click "add references", navigate to "projects/solution" on the left bar, add "aktos_dcs_cs"
4. In your project, add `using aktos_dcs_cs;` line at the top of the file. 
5. Create any number of classes which are inherited from `Actor`
6. Do your blocker works in `public override action(){ ... }` method
7. Send any message to others via `send(object)` method
8. Receive others' messages via `public override receive(message){ ... }` method
9. Receive others' messages via `public handle_SUBJECT(message){...}` methods if you know the subject. 
10. DO_NOT perform blocker operations in receiver methods. 
11. Initialize your objects from these `Actor` based classes
12. Add `Actor.wait_all()` at the end of program
13. See your objects are messaging to each other. 
