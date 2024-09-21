# Project Overview
This project consists of several scripts that work together to manage and render captions with customizable text properties in Unity. Below is a summary of each script and its functionality.\
For a full project overview and tutorial, please see:

## Getting Started
1.	Setup: Import the project into Unity.
2.	Prefabs: Ensure the CaptionPrefab is assigned in the CaptionManager.
3.	Run: Play the scene to see the caption rendering and text customization in action.

## Data Structure & Terminology 

### Cue
A **cue** represents a specific segment of text that is displayed at a particular time during a video (i.e. a caption). It includes information about when the text should appear and disappear, as well as the text content itself. 

### Cue Group
A **cue group** is a collection of cues, such as "Music", "Speech", or "Sound Effects". Cue groups allow for easier "batch" processing and editing of captions, such as having all music captions in red at the bottom left corner.\
Note: Only a single cue from a specific cue group can be shown at once. E.g. you can have a "Music" and "Speech" cue overlap in time, but two "Music" cues cannot overlap in time. 

### Element 
**Elements** are abstracted "tags" that can be placed on whole cues or specific words or characters. E.g. Using html syntax for demonstration, "I <mood=excited> really love you </mood>", `mood` would be the element

### Setting
Settings define how the text or region should be displayed, such as alignment, size, color, and other visual properties.\
For example, A setting might specify that the text should be bold, aligned to the center, or have a specific font size.

### Regions
Regions are special settings that govern where on the screen the text box should be anchored and other properties such as the background color of the text box, how many lines can be displayed, etc. \
Note: Regions can only be applied to an entire cue and cannot be applied to specific words

### Feature
A feature is a collection of settings that can be applied together to a cue. Features group related settings to simplify the application of multiple settings at once. In the CaptionRenderer class, features are retrieved from styles and applied to cues and regions.\
For example, a feature might include settings for text alignment, font size, and color, which together define a specific style for a caption.

### Mappings
A mapping connects an element to a feature. Mappings allow for some basic logic, e.g. "If mood == happy, then use the `Rainbow` feature"

### Mapping Group
A collection of mappings saved together as a single preset that allows users to change between mappings quickly

### How It All Comes Together
1.	Cues are the individual pieces of text that need to be displayed at specific times.
2.	Settings define the visual properties of these cues and the regions they are displayed in.
3.	Features group multiple settings together, making it easier to apply a consistent style to cues and regions.
4.	Elements are abstract tags placed on the captions themselves.
5.	Mappings connect elements to features.
6.	Mapping groups save a set of mappings together as a single unit for ease of use.
   
## Main Scripts

### CaptionManager.cs
Purpose: Manages the creation and handling of caption renderers.\
Key Features:\
•	Singleton pattern to ensure a single instance.\
•	Provides a method to create new CaptionRenderer instances using a prefab.

### TextColorAndSizeChanger.cs
Purpose: Changes the color and size of each character in a TextMeshProUGUI component.\
Key Features:\
•	Initializes the TextMeshProUGUI component and sets initial text.\
•	Modifies the vertex colors and sizes of text characters based on their index.\
•	Continuously updates the text mesh to reflect changes.

### CaptionRenderer.cs
Purpose: Handles the rendering of captions.\
Key Features:\
•	Manages the display and formatting of caption text.\
•	Integrates with TextMeshProUGUI for advanced text rendering.

### ColorInput.cs
Purpose: Handles user input for color selection.\
Key Features:\
•	Provides a user interface for selecting colors.\
•	Updates the text color based on user input.

### Common.cs
Purpose: Contains common utilities and constants used across the project.\
Key Features:\
•	Provides helper methods and constants for consistent usage.

### MarkupFormatter.cs
Purpose: Formats text with markup for special effects.\
Key Features:\
•	Adds markup tags to text for styling.\
•	Supports various text effects like bold, italic, and color changes.

### MouseClickHandler.cs
Purpose: Handles mouse click events.\
Key Features:\
•	Detects mouse clicks and triggers corresponding actions.\
•	Can be used to interact with text elements or other UI components.
