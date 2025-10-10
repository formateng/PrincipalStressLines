<img width="160" height="100" alt="LilyPadHighRes_1" src="https://github.com/user-attachments/assets/63df58e0-86d6-4edc-b7c5-086aae9834c2" />


# LilyPad
A Grasshopper plug-in which generates principal stress lines for optimised plate, slab, and shell structures.

There are two main advantages to this plugin over other stress line modelling methods (e.g. Karamba's Line Results Component): The ability to use the displacment field from higher order elements for improved accuracing of the principal stress directions and the usage of seeding methods to automate the process of selecting stress lines which are evening spaced creating a cleaner plot.

The plug-in orginated from the [Master's Thesis of Matthew Church at TU Delft](https://repository.tudelft.nl/record/uuid:b35b0984-1fbb-4f18-81d3-a01a8c9c8060) conducted between 2020 and 2022. This original work explored the best practices for generating highly accurate and evenly spaced stress lines on planar slab and plate geometries. Format Engineers have developed the plugin futher by enabling stress lines to be generated on non-planar meshes, developed the displacement field to work with triangular faces, and improved the user experience to make it more intuitive.

### Overview
The process of generating the stress lines is broken down into several steps to allow the user to control the method for generating the stress lines. The key parts to this are in determining how the field which generates the stress vectors is created, and the method for seeding the new start points of stress lines.

This flow chart visually explains the process of creating stress lines and the points at which the user can determine the methods.
<img width="960" height="540" alt="Lilypad Flow Diagram_Rev01" src="https://github.com/user-attachments/assets/d9d47497-ebc3-4e3e-b815-11e84a7e9fba" />

## References and Dependancies
Lilypad implements methods developed by the following authors: 
+ [Tam, K.M.M., 2015. Principal stress line computation for discrete topology design](https://dspace.mit.edu/handle/1721.1/99630)
+ [Jobard, B. and Lefer, W., 1997. Creating evenly-spaced streamlines of arbitrary density](https://link.springer.com/chapter/10.1007/978-3-7091-6876-9_5)
+ [Mebarki, A., Alliez, P. and Devillers, O., 2005, October. Farthest point seeding for efficient placement of streamlines](https://ieeexplore.ieee.org/abstract/document/1532832)

Lilypad uses the Triangle.NET library:
+ [Christian Woltering, et al., Triangle.NET](https://github.com/wo80/Triangle.NET)


## Licencing
Although LilyPad is provided as open source with an MIT license, the code is dependant on Triangle.NET which is based on Jonathan Richard Shewchuk's Triangle project. As a result there is ambiguity around how a derived work should be handled in regard to commerical licencing. Please refer to this [discussion](https://github.com/wo80/Triangle.NET/discussions/50) on Triangle.NET for more details.

## Development Tracker
LilyPad is still under development. This outlines the development progress so far.

+ 🔳Field Generation
  + ✅Vector Field
    + ✅Implimentation of Tam (2015) field method.
  + ✅Displacement Field
    + ✅Tri 3 Elements
    + ✅Quad 4 Elements
    + ✅Tri 6 Elements
    + ✅Quad 8 Elements
    + ✅Single Component for Standard (Tri 3 and Quad 4) elements.
    + ✅Allow for axial and bending principle stress in Standard Elements.
    + 🔳Single Component for Higher (Tri 6 and Quad 8) elements.
    + 🔳Allow for axial and bending principle stress in Higher Elements.
+ ✅Stress Line Generation
    + ✅Euler Integration
    + ✅Runge-Kutta 4 Integration
    + ✅Detect 180 degree flipping
    + ✅Adaptive stepping
    + ✅Loop detection
    + ✅Loop closure
    + ✅Detection of proximity to other stress lines.
+ Seeding Methods

## FAQs
