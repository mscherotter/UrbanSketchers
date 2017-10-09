# Urban Sketchers
Work in Progress by Michael S. Scherotter

Urban Sketchers Mobile App will map urban sketches from around the world.

## Sketch
Each *Sketch* has the following properties
* Title (String)
* Description (Optional String)
* Address (Optional String)
* Latitude (double)
* Longitude (double)
* Image Url (String)
* Creation Date (Date/time)
* Created By (Person)

## Person
Each *Person* has the following properties
* Name (String)
* Image Url (String)
* Public Url (String)

## Rating
Each *Rating* has the following properties
* Sketch (String)
* Person (String)
* Like (Boolean)
* Comment (String)
* Is Violation (Boolean)