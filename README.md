# HCI Project - Head's Oscillations Simulator

## Author
[Etienne VATRY](etienne.vatry@stud-mail.uni-wuerzburg.de)

## Supervisor
[Dr. Jean-Luc Lugrin](jean-luc.lugrin@uni-wuerzburg.de)

# Head's Oscillations Simulation - Project Report

## Project Overview

This project aims to create a simulation to measure the impact of head oscillations on user comfort in virtual reality environments. The simulation includes three distinct virtual environments with varying terrain types and features that allow for comprehensive testing and analysis.

## Project Structure

The project's main assets, including scenes, scripts, and animations, are located in the following directory: ``vatry-hci-project/Assets/Scenes/HeadOscillations``


## Features

- **Three Virtual Environments**: 
  - Flat terrain
  - Terrain with regular bumps
  - Terrain with noise-generated bumps

- **Head Oscillations**: Ability to activate or deactivate head oscillations during user movement.

- **Footstep Sounds**: Integration of footstep sounds that can be activated or deactivated during oscillations.

- **Real-Time Monitoring**: Fast Motion Sickness scales appear at regular intervals to monitor user discomfort in real-time.

- **Data Logging**: Results of the comfort tests are saved in a text file for further analysis.

## Controls

The controls for the simulation are as follows:
- **Walk**: Right OR left trigger button
- **Run**: Right AND left trigger button
- **Increase slider value during test**: Right trigger button
- **Decrease slider value during test**: Left trigger button
- **Validate test**: A button
- **Activate/deactivate oscillations**: Right grip button
- **Activate/deactivate footstep sound during oscillations**: Left grip button (note: footstep sound can only be heard when oscillations are activated)

![Quest Touch Controllers](vatry-hci-project/Assets/Scenes/HeadOscillations/quest_controls.png)

## Getting Started

To get started with this project, follow these steps:

1. **Clone the repository**:
    ```sh
    git clone <repository_url>
    ```

2. **Open the project in Unity**:
   - Ensure you have Unity installed.
   - Open Unity Hub and add the cloned project.

3. **Navigate to the main scenes**:
   - Go to `Assets/Scenes/HeadOscillations`.
   - Open the scene files in Unity to start exploring and running the simulation.

## Usage

1. **Run the Simulation**:
   - Load one of the scene files in Unity.
   - Press the play button to start the simulation.

2. **Interact with the simulation** using the controls outlined above.

3. **Monitor User Comfort**:
   - During the simulation, Fast Motion Sickness scales will appear at regular intervals.
   - User responses are logged for analysis.

## Conclusion

This project provides a robust platform for testing and understanding the impact of head oscillations on user comfort in virtual reality. The modular design allows for easy adaptation and expansion for future research and development.

## Acknowledgements

I would like to express my sincere gratitude to Dr. Jean-Luc Lugrin, my supervisor, for his invaluable help, advice, and support throughout this project.

## License

This project is licensed under the [MIT License](LICENSE).
