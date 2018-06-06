# Vector Search project

 This is a research project analysing various approaches in which an entity/entities can be searched in a big collection of data contexts.
 Initially the scope of this project is limited only to build simple classes that could provide better search performance against full-scans. The direction of this project will be evolving based on the progress
 new goals will be set. Ok lets get into bussiness, What is a Full-scan search?? full-scan is an approach of scanning each and every items in a collection from top to bottom against our input. 
 Let's take a real-world scenario where we have to find five cars which share same or similar specs of a candidate car, or another scenario would be to display the best range of mobile phones based on input criteria like price, CPU, memory etc.
 In both these scenarios, we can populate the result by matching the input candidate with all other entities in the collection, by equating their properties or comparing the properties against a range. 
 This kind of searching approach is called Fullscan search, full-scan is fine if the collection is considerably small, say a 100 - 1000 entities in a collection.
 But as collection increases the time complexity (time complexity = n) of the process will be increased linearly, hence this approach is not ideal in the case of 100K or 5M entity collection.
 So we have to consider about approaches that can reduce the time complexity and improve performance. An alternative way for searching items in a collection against the full scan method is Vector method.
 In this method, all entities are treated as vectors and their properties/attributes are taken as coordinates of the same.
 The vectors are plotted/arranged in a vector space (Euclidean plain) the closest the vectors are, they share similar property. In real-world scenarios,
 these vectors can be any entity say a car, person, mobile phone etc, and their properties can be marked as coordinates in an N-dimensional vector space. 
 
 ### Three methods are available at this stage 
 
 Nearest : Find the nearest entities/vectors to the input.

 Exact : Find the vectors which exactly share same vector point.

 Find : Find other vectors in the plain, based on the available values of input coordinates.

 These methods use hashing and some maths to perform the search, detailed document will be prepared and updated. Download the Codebase and go through the samples, method names are self explainary and comments are added
 where as possible.
 

What things you need to install the software and how to install them

## Integration

This project is build under .Net Framework 4.0 client profile so it will integrate with projects that are build using .Net framework 4.0 and above.
This library have no external dependencies so hussile free integration, download the dll from the build folder under the master branch and make a reference to the dll 
in you project you are good to go.

##Future

Currently this project can compute numerical data only, in next stages the application will be able to perform text based searches too..