<div align="center">

  <img width="200" height="auto" alt="logo" src="https://github.com/user-attachments/assets/bcc4dd2a-aaac-4f24-a98e-73edede9dd47" />
  <h1>SimulEase - A VR-Assisted Approach to Overcoming Social Anxiety</h1>
  
  <p>
   A project developed for my Master Degree thesis, at University of "Alexandru I. Cuza", Faculty of Computer Science, Software Engineering Master.
  </p>
  
</div>

<br />

<!-- Table of Contents -->

# :notebook_with_decorative_cover: Table of Contents

- [About the Project](#star2-about-the-project)
  - [Screenshots](#camera-screenshots)
  - [Tech Stack](#space_invader-tech-stack)
  - [Features](#dart-features)
  - [Environment Variables](#key-environment-variables)
- [Usage](#eyes-usage)
- [License](#warning-license)
- [Contact](#handshake-contact)

<!-- About the Project -->

## :star2: About the Project

One of the most common mental illnesses in the world, anxiety disorders can significantly impair a person's ability to go about their daily life, interact with others, or calm oneself in everyday situations like speaking in front of an audience or interacting with people.   Despite their effectiveness, conventional treatment approaches like cognitive behavioral therapy (CBT) may be limited by stigma, accessibility, or the user's capacity to handle challenging circumstances in day-to-day life.

Our goal in developing SimulEase is to use interactive technology to bridge the gap between therapy intent and actual practice.  By simulating anxiety-inducing situations in a safe, regulated virtual reality (VR) setting, users can gradually develop tolerance and learn coping mechanisms without having to face the repercussions of their actions in real life. The aim of this research and thesis is to create and evaluate SimulEase, a virtual reality application that simulates anxiety-inducing real-life scenarios in a safe, regulated, and repetitive environment.  The software aims to help users better manage the symptoms of social anxiety and stress related to public speaking by using immersive exposure and AI stress detection (from user input in the form of text and audio).

SimulEase is a virtual reality (VR) program created in Unity that helps people with social anxiety by exposing them to more difficult social settings in a virtual setting.  According to the tenets of exposure therapy, the method aims to gradually acclimate people to social situations that make them feel afraid by exposing them to progressively more difficult interacting situations.  Because these scenarios are specifically made to resemble real-world social settings, users can engage in experiential learning in a safe and regulated virtual setting.  The system mimics real-world social circumstances and provides users with interactive practice by combining a number of cutting-edge technologies, including realistic 3D animation, AI-based conversational bots, and speech recognition/synthesis. The system architecture is modular and distributed, with three core subsystems: the VR world, the AI conversational interface agent, the emotion recognition AI service and the speech interaction platform. These communicate predominantly through RESTful APIs to enable real-time responsiveness and scalability.

<!-- Screenshots -->

### :camera: Screenshots

<div align="center"> 
  <img width="1042" height="545" alt="cafe-scene" src="https://github.com/user-attachments/assets/4055be51-f0b0-4c96-84ca-3bacc4d9b00e" />
  <img width="469" height="418" alt="office" src="https://github.com/user-attachments/assets/cf7afdf0-c69c-4837-9bee-a1852667326f" />
  <img width="442" height="408" alt="venue" src="https://github.com/user-attachments/assets/2ed95f12-4ad9-4d39-a779-d735c76d19c7" />
  <img width="905" height="923" alt="SimulEase - People" src="https://github.com/user-attachments/assets/c1127980-972d-4306-8295-cf35a4251e56" />
</div>

<!-- TechStack -->

### :space_invader: Tech Stack

<details>
<summary>User Interface</summary>
  <ul>
  <li><a href="https://unity.com/">Unity</a></li>
  <li><a href="https://www.blender.org/">Blender</a></li>
  <li><a href="https://www.mixamo.com/">Mixamo</a></li>
  <li><a href="https://www.reallusion.com/accuRIG/">AccuRIG</a></li>
  <li><a href="https://renderpeople.com/">Renderpeople</a></li>
  <li><a href="https://assetstore.unity.com/">Unity Asset Store</a></li>
  <li><a href="https://www.cgtrader.com/">CGTrader</a></li>
</ul>
</details>

<details>
<summary>AI Services</summary>
  <ul>
    <li><a href="https://azure.microsoft.com/en-us/products/cognitive-services/speech-services/">Azure Speech</a></li>
  <li><a href="https://www.microsoft.com/en-us/security/business/ai/azure-ai-studio">Azure Foundry (OpenAI Integration)</a></li>
  <li><a href="https://www.hume.ai/">Hume AI</a></li>
  </ul>
</details>

<!-- Features -->

### :dart: Features

* **Voice-Based Emotion Detection**: Captures users’ voice input in real-time and analyzes it using emotion recognition models such as Hume API or custom NLP logic.

* **Flexible Emotion Source Integration**: Supports multiple emotion detection backends (e.g., HumeEmotionListener or ContinuousSpeechRecognizer) through a shared interface.

* **Real-Time Emotion Tracking**: Continuously records emotional responses during the session and aggregates them for post-experience analysis.

* **Interactive Countdown Experience**: Timed immersive session using a dynamic countdown timer, including smooth fade transitions and visual guidance.

* **Emotion Frequency Logging**: Stores and tracks emotional responses over time, calculating frequency and intensity for later visualization.

* **Session Statistics Visualization**: Displays user emotions as interactive bar charts, with percentage breakdowns and dynamic grouping of less frequent emotions.

* **Emotion History Summary**: Saves emotion session summaries for reflection, analysis, or future feature use like trend tracking or user journaling.

* **Pause and Resume Support**: Built-in session control with user-friendly pause/resume functionality for flexibility and comfort during immersive experiences.

* **Multi-Platform VR Compatibility**: Designed to support XR systems using Unity’s XR Interaction Toolkit and OpenXR (e.g., Oculus/Quest/PC VR).

* **User-Centered Design**: Intuitive VR interface focused on accessibility and immersion, including minimal UI interference and gaze-based or controller input options.


<!-- Env Variables -->

### :key: Environment Variables

To run this project, you will need to add a config.js file inside the scripts/ folder with your service credentials and API settings.

```json
{
  "subscriptionKey": "",
  "region": "westeurope",
  "openAiKey": "",
  "openAiEndpoint": "https://-eastus2.cognitiveservices.azure.com/",
  "deployment": "gpt-4",
  "apiVersion": "2025-01-01-preview",
  "humeApiKey": ""
}

```
📁 Location: Place this file at Assets/Scripts/config.json.

<!-- Usage -->

## :eyes: Usage

After installation, the user is directed to the Scenario Library, a collection of terrifying virtual situations created using the concepts of graded exposure therapy.  The scenarios include performing a job interview, striking up a discussion in a social situation, such as a coffee shop, or standing in front of a large or small group.  The exposure period and difficulty can be adjusted by the user, who can select a scenario. Static or less severe exposure modes are suitable for initial acclimatization.

The user will be immersed in a complete virtual reality experience after the selected scenario has been activated.  AI emotion recognition is used to continuously monitor the session and ensure that the user is enjoying the SimulEase scene in a safe manner.  The software provides immediate coping mechanisms in situations of extreme stress, such as grounding techniques, guided breathing, or even stopping the simulation if the anxiety becomes intolerable.  This adaptive approach aligns with studies showing that real-time biofeedback can improve exposure-based treatment outcomes \cite{reiner2008biofeedback}.  After every session, a performance report is given to the user.  This include data like the length of the session, metrics for speech performance (such hesitation or emotional condition), and suggestions for improvement. All these give feedback to the user to reflect on their improvement and know their anxiety patterns better.

When the app first launches, the user selects a scenario and degree of difficulty, as seen in the diagram below. From there, they can enter one of the personalized experiences.  Affective monitoring systems, branching logic that depends on user input, and dynamic ambient elements (such AI interactions or animated crowds) are all included in each scenario.  If symptoms of anxiousness are detected (e.g., through affect detection or eye contact), the provision of help prompts and optional breaks is launched.  The flow allows the user to self-regulate, encourages either momentary or continuous disengagement, and ends with an application exit or scenario completion.

<img width="2091" height="1009" alt="user-journey-diagram" src="https://github.com/user-attachments/assets/39ab188b-180c-4883-8839-840e39996fdf" />



1. **The Office Scene**:

One of SimulEase's primary virtual environments, the Office Presentation Scene, is intended to evoke social anxiety related to the job, such as performance reviews and public speaking in front of coworkers.  The user in this instance is standing at the head of a conference table, getting ready to present to ten seated virtual characters.  These characters' body language and spatial arrangement have been purposefully designed to conjure subtle and realistic sources of social pressure, which is in line with the cognitive-behavioral model of social anxiety, which places an emphasis on negative evaluation and perceived judgment.The characters engage in a variety of socially offensive actions to heighten the environment's immersive, anxiety-inducing qualities.  The feeling of being watched is further enhanced by the fact that many avatars keep direct eye contact or clearly look the person over from head to toe.  During the presentation, one figure appears disengaged, typing on a laptop, displaying inattention, a social anxiety disorder that is frequent in people with social anxiety.  Positioned at the end of the table and portrayed as a "boss" figure, another avatar gently communicates time pressure and potential judgment by periodically checking their watch with an obvious look of irritation.  Furthermore, a character that appears as an office coworker and is visible via a glass wall passes by the office door at random while making loud footsteps, which raises the risk of unplanned outside disruptions.

2. **The Venue Scene**:

SimulEase's second scene is designed to replicate the high-stress sensation of public speaking in front of an audience, which is another thing that causes social anxiety for most individuals.  In the Large Venue Presentation Scene, the user takes the stage in front of a sizable auditorium that has roughly 200 virtual guests.  To create a realistic and socially dynamic environment, the audience is intended to be seated and semi-interactive, although softly animated.  Avatars mimic a variety of audience actions that socially anxious people may interpret as critical or disengaged, such as shifting in their seats, leaning to whisper to one another, and occasionally appearing inattentive.  Ambient sound plays a crucial role in enhancing the experience's immersive realism.  There is a low murmur of conversation among the people in the venue, the ringing of cell phones, finger tapping on touchscreens, and the odd microphone static, all of which introduce a feeling of unpredictability and exposure. A random scripted animation that shows a woman getting up in the middle of a session and leaving the auditorium adds to the environment's unpredictable nature.  This scene is meant to mimic a prevalent catastrophic interpretation in socially anxious people, which holds that their failure is directly reflected in their separation from others.

3. **The Cafe Scene**:

The Cafe Conversation Scene is the most socially involved environment in our SimulEase project. It is intended to mimic a casual social context and addresses anxieties about small talk, paying attention, and interacting with people one-on-one.  In this particular scenario, the user is seated across from a virtual character named Alina at a cafe table. Alina is an AI-driven conversational partner that mimics the user's buddy by reacting contextually to the user's speech and actions.  This arrangement reflects a typical situation for people with social anxiety disorder (SAD), which can be a significant source of distress due to avoidance of negative appraisal or perceived social assessment. In order to increase realism and interest, Alina is created to display realistic idle behaviors like blinking and slight head movements. Additionally, her speech is supported by expressive gestures and animated lip-syncing.  There are other virtual people in the cafe itself, which adds richness to the ambient setting.  There are active waitstaff moving through the room at random intervals, characters conversing at adjacent tables, and one person working quietly on a laptop.  While adding small distractions to simulate a real-world environment, these background features support the simulation's ecological validity and immersion. This scene is unique because it emphasizes subtle nonverbal social interactions and real-time behavioral feedback.  A soft on-screen cue draws the user's attention to refocus their gaze after they had avoided eye contact with Alina for a considerable amount of time, which is a common response among socially nervous people who attempt to avoid being seen.  This encourages users to accept the unpleasantness of direct interpersonal attention without avoiding it, acting as a cue for corrective behavior as well as a mechanism for exposure to anxiety-inducing stimuli.

### **Demo**

Check out our live demo to see SimulEase in action: [Youtube SimulEase Demo](https://youtu.be/qqRoeoP07iE)
<!-- License -->

## :warning: License

Distributed under the no License.

<!-- Contact -->

## :handshake: Contact

Ungureanu Delia Elena - deliaungureanu2001@yahoo.com
