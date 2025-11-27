<img width="160" height="100" alt="LilyPadHighRes_1" src="https://github.com/user-attachments/assets/63df58e0-86d6-4edc-b7c5-086aae9834c2" /> 

# LilyPad
A Grasshopper plug-in which generates principal stress lines for optimised plate, slab, and shell structures.

There are two main advantages to this plugin over other stress line modelling methods (e.g. Karamba's Line Results Component): The ability to use the displacement field from higher order elements for improved accuracy of the principal stress directions and the usage of seeding methods to automate the process of selecting stress lines which are evening spaced creating a cleaner plot.

The plug-in originated from the [Master's Thesis of Matthew Church at TU Delft](https://repository.tudelft.nl/record/uuid:b35b0984-1fbb-4f18-81d3-a01a8c9c8060) conducted between 2020 and 2022. This original work explored the best practices for generating highly accurate and evenly spaced stress lines on planar slab and plate geometries. Format Engineers have developed the plugin further by enabling stress lines to be generated on non-planar meshes, developed the displacement field to work with triangular faces, and improved the user experience to make it more intuitive.

<img width="200" height="200" alt="Animation of Stress lines on a Lily Pad Shape" src="https://github.com/user-attachments/assets/a7686f8a-f038-4c99-a989-b3f73f759a08" />
<img width="200" height="200" alt="Image of Stress lines produced in example 0" src="https://github.com/user-attachments/assets/183d6bcf-6a57-4df8-8021-e3522424aa01" />
<img width="200" height="200" alt="Image of Stress lines produced in example 1" src="https://github.com/user-attachments/assets/e1c970b2-4cd3-4dc0-b592-77b948ff0817" />
<img width="200" height="200" alt="Image of Stress lines produced in example 2" src="https://github.com/user-attachments/assets/7df288d9-f628-4585-9fee-d80d41ea3677" />
<img width="200" height="200" alt="Image of Stress lines produced in example 3" src="https://github.com/user-attachments/assets/0320ea3e-a079-4f2b-bc79-f4288c6e9f4b" />



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

+ ✅Field Generation
  + ✅Vector Field
    + ✅Implementation of Tam (2015) field method.
  + ✅Displacement Field
    + ✅Tri 3 Elements
    + ✅Quad 4 Elements
    + ✅Tri 6 Elements
    + ✅Quad 8 Elements
    + ✅Single Component for Standard (Tri 3 and Quad 4) elements.
    + ✅Allow for axial and bending principle stress in Standard Elements.
    + ✅Single Component for Higher (Tri 6 and Quad 8) elements.
    + ✅Allow for axial and bending principle stress in Higher Elements.
+ ✅Stress Line Generation
    + ✅Euler Integration
    + ✅Runge-Kutta 4 Integration
    + ✅Detect 180 degree flipping
    + ✅Adaptive stepping
    + ✅Loop detection
    + ✅Loop closure
    + ✅Detection of proximity to other stress lines.
    + 🔳 Listen for ESC press and break loop
    + 🔳 Runtime Optimisation
    + 🔳 Run Stress Line Component as parallel for multiple seed inputs
+ Seeding Methods
    + ✅Implement Neighbour Seedind Method
    + 🔳Implement Farthest Point Seedin Method
      + 🔳 Updated TriangleNET package and methods based on this
      + 🔳 Create implementation of this method for non-planar meshes
    + 🔳 Listen for ESC press and break loop
    + 🔳 Runtime Optimisation

## FAQs
**Where do I start?**
  - Download the example files from Releases (or Food4Rhino) and look through what can be done?

**Can this only be used with Karamba or GSA?**
  - No, this can in theory work with any Finite Element Analysis software. The difficulty is with getting the neccessary data into Grasshopper. We'd recommend that you use the GSA example files as a start and see how to get your results into the same format.

**What's a principal stress line?**
  - If you're unaware of stress lines the best place to start is [Mohr's circle](https://en.wikipedia.org/wiki/Mohr%27s_circle#Finding_principal_normal_stresses).

**How does this compare with Karamba's Line Results Component**
  - The major benefit of this plugin is that an entire field of evenly spaced stresslines can be generated in one go with the user needing to pick seeding points. Additionally, using the higher order elements generally gives cleaner results. But currently LilyPad is slower at producing stresslines.

**Should I create in-plane or bending principle stress lines?**
 - If your interested in effects resulting from axial forces inside the material then use in-plane stress lines (for example, forces in shear walls), if you're interested in effects resulting from bending forces then use bending stress lines (for example, optimal beam placement in a slab).

**Why do stress lines stop when they get close to others?**
  - This is done to create clear images. If you would like your stress lines to always continue tell they meet the edge or loop then set dTest to zero.

**Why doesn't this work everytime?**
  - We're releasing this as a pre-release before we've fixed all the bugs so that others can start to use the tool. As we continue to develop the plugin we'll release updates on the Package Manager.

**Is LilyPad Avaliable for Rhino 7?**
  - At present LilyPad is only published to the Rhino 8 Package Manager.

**There's so many ways to generate the stress lines, which is best for my case?**
  - This is impossible to say and will depend on your geometry, loading and how long you want the computer to run for. If you can use higher order elements in your analysis software this generally gives the cleanest results as the principle vector field is smoothest.

**Why do I need to input the poison's ratio**
  - Although the effect can be small, it does have a influence on the mathematics.
