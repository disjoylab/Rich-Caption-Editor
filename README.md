# Project Overview
This project consists of several scripts that work together to manage and render captions with customizable text properties in Unity. Below is a summary of each script and its functionality.

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

## Getting Started
1.	Setup: Import the project into Unity.
2.	Prefabs: Ensure the CaptionPrefab is assigned in the CaptionManager.
3.	Run: Play the scene to see the caption rendering and text customization in action.
   
## Usage
•	CaptionManager: Use this to create and manage captions.\
•	TextColorAndSizeChanger: Attach this to a GameObject with a TextMeshProUGUI component to see dynamic text changes.\
•	ColorInput: Use this for user-driven color changes.\
•	MouseClickHandler: Attach this to handle mouse interactions.

