# TopologicalOrdering
[University Project]

[2nd Year - 2nd Semester]

Objective: Order V elements based on E relations (between pairs of elements)

Language: C++  
Efficiency: O(V + E) - linear  
Method: Depth First Search (DFS).  

A relation is a pair of elements u, v representing element u would be before element v if all the elements were in order.

The input consists of:
- one line with the number of elements to order (N) and the number of relations L (between a pair of elements) known;
- a list with N lines of relations (between a pair of elements);

The output is one of three possible:
- "Insuficiente" (Insufficient): the input is not sufficient to find a unique order for the elements;
- "Incoerente" (Incoherent): the input is not coherent. Inconsistent relations;
- A line of N elements written in order (based on the relations given as input).
