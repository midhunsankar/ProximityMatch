# Vector Search project

 An alternative way for searching items in a collection against the full scan method. In this method all entities are treated as vectors and their properties/attributes are taken as coordinates of the same.
 The vectors are ploted/arranged in an vector space (Eucledian plain) the closest the vectors are, they share similar property. In realworld scenarios these vectors can be any entity say a car, person, mobile phone etc, 
 and their properties can be mark as coordinates in an N dimentional vector space. Lets take an real word scenarion where we have to find five cars which share same or similar specs of a candidate car, or an other scenario would
 be to show the best range of mobile phones based on an input criterias like price, cpu, memmory etc. In both these scenarios we can populate the result by matching the input candidate with all other etities in the collection,
 by equating their properies or comparing the properies against a range. This kind of searching approch is called Fullscan search, fullscan is fine if the collection is considerably small, say a 100 - 1000 entities in a collection.
 But as collection increases the time complexity (timecomplexity = n) of the process will be increased linearly, hence this approch is not ideal in the case of 100K or 5M entity collecion. 

  either comparing  
   one million cars   

### Prerequisites

What things you need to install the software and how to install them

## Integration

This project is build under .Net Framework 4.0 client profile so it will integrate with projects that are build using .Net framework 4.0 and above.
This library have no external dependencies so hussile free integration, download the dll from the build folder under the master branch and make a reference to the dll 
in you project you are good to go.