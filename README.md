# Vector Search project

An alternative way for searching items in a collection against the full scan method. In this method, all entities are treated as vectors and their properties/attributes are taken as coordinates of the same. 
The vectors are plotted/arranged in a vector space (Euclidean plain) the closest the vectors are, they share similar property. 
In real-world scenarios, these vectors can be any entity say a car, person, mobile phone etc, and their properties can be marked as coordinates in an N-dimensional vector space. 
Let's take a real-world scenario where we have to find five cars which share same or similar specs of a candidate car, or another scenario would be to display the best range of mobile phones based on input criteria like price, CPU, memory etc. 
In both these scenarios, we can populate the result by matching the input candidate with all other entities in the collection, by equating their properties or comparing the properties against a range.
 This kind of searching approach is called Fullscan search, full-scan is fine if the collection is considerably small, say a 100 - 1000 entities in a collection.
 But as collection increases the time complexity (time complexity = n) of the process will be increased linearly, hence this approach is not ideal in the case of 100K or 5M entity collection. 

 Time complexity = O(d) + O(n/d) + O(1)
 

### Prerequisites

What things you need to install the software and how to install them

## Integration

This project is build under .Net Framework 4.0 client profile so it will integrate with projects that are build using .Net framework 4.0 and above.
This library have no external dependencies so hussile free integration, download the dll from the build folder under the master branch and make a reference to the dll 
in you project you are good to go.