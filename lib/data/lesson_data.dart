import 'package:flutter/material.dart';
import 'package:vitalact/models/steps/reading_step.dart';
import 'package:vitalact/models/steps/multi_choice_step.dart';
import 'package:vitalact/models/steps/text_input_step.dart';
import '../models/lesson_item.dart';

class PlaceholderPage extends StatelessWidget {
  final String title;
  const PlaceholderPage({super.key, required this.title});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(title)),
      body: const Center(child: Text("Lesson Content Here")),
    );
  }
}

final List<LessonItem> lessonData = [
  /*

      LESSON 1

  */
  LessonItem(
    title: 'Breathing Assessment',
    spriteAsset: 'assets/spritesheet/NVSA.png',
    steps: [
      const ReadingStep(
        id: 'What-is-normal',
        title: 'What Is Normal?',
        content: '''
Normal Breathing
 • 12–20 breaths per minute (adult)
 • Regular rhythm
 • Chest rises and falls evenly
 • Can speak full sentences without stopping


Note:
Normal → No immediate danger
Not Normal → Something is wrong
Concerning → Monitor closely
Emergency → Immediate action needed''',
      ),
      MultiChoiceStep(
        id: 'Q1',
        title:
            'Person breathing 16/min, regular rhythm, chest rising evenly, speaking full sentences.',
        instructions: 'Choose the best answer.',
        spriteAsset: 'assets/spritesheet/BreathingO.png',
        choices: const ['Normal', 'Not Normal'],
        correctIndex: 0,
        disclaimer:
            'Assume this is a sudden situation and the person was previously stable.',
        correctExplanation:
            "All key signs are normal: breathing rate is within 12–20, rhythm is regular, chest movement is even, and the person can speak clearly.",
        incorrectExplanation:
            "All signs match normal breathing: correct rate, steady rhythm, even chest movement, and ability to speak clearly..",
        hint:
            "Check all signs: rate, rhythm, chest movement, and speaking ability.",
      ),
      MultiChoiceStep(
        id: 'Q2',
        title: 'Person breathing 18/min but cannot speak full sentences.',
        instructions: 'Choose the best answer.',
        spriteAsset: 'assets/spritesheet/BreathingO.png',
        choices: const ['Normal', 'Not Normal'],
        correctIndex: 1,
        disclaimer:
            'Assume this is a sudden situation and the person was previously stable.',
        correctExplanation:
            "Even with a normal breathing rate, difficulty speaking indicates breathing is not functioning normally.",
        incorrectExplanation:
            "If a person cannot speak full sentences, their breathing is not normal even if the rate looks fine.",
        hint: "Normal breathing should allow the person to speak comfortably.",
      ),
      MultiChoiceStep(
        id: 'Q3',
        title: 'Person breathing 21/min, regular rhythm, speaking clearly.',
        instructions: 'Choose the best answer.',
        spriteAsset: 'assets/spritesheet/BreathingT.png',
        choices: const ['Normal', 'Concerning', 'Emergency'],
        correctIndex: 1,
        disclaimer:
            'Assume this is a sudden situation and the person was previously stable.',
        correctExplanation:
            "Breathing above 20/min is outside the normal range, but since other signs are stable, it is concerning—not an emergency.",
        incorrectExplanation:
            "The rate is too fast to be normal, but there are no severe danger signs, so it is not an emergency.",
        hint: "Pay attention to the breathing rate range (12–20).",
      ),
      MultiChoiceStep(
        id: 'Q4',
        title: 'Breathing 14/min, chest not rising, cannot speak.',
        instructions: 'Choose the best answer.',
        spriteAsset: 'assets/spritesheet/BreathingT.png',
        choices: const ['Normal', 'Concerning', 'Emergency'],
        correctIndex: 2,
        disclaimer:
            'Assume this is a sudden situation and the person was previously stable.',
        correctExplanation:
            "Even though the rate appears normal, the chest is not rising and the person cannot speak, meaning breathing is not effective — this is an emergency.",
        incorrectExplanation:
            "Without chest movement and the ability to speak, the person is not breathing properly, which is life-threatening.",
        hint: "Breathing must include chest movement and ability to speak.",
      ),
      const TextInputStep(
          id: 'Q5',
          instructions: 'Describe the person’s breathing condition?',
          title:
              'Person breathing 20/min, regular rhythm, chest rising evenly, but the person cannot speak full sentences without stopping.',
          aiPrompt: 'Give a brief explanation of the answer',
          spriteAsset: 'assets/spritesheet/BreathingO.png',
          hint: 'Check if all normal breathing signs are present.')
    ],
  ),
  /*

      LESSON 2

  */
  LessonItem(
    title: 'Consciousness Check',
    spriteAsset: 'assets/spritesheet/RedFlags.png',
    steps: [
      const ReadingStep(id: ' R1', title: 'What Is Consciousness?', content: '''
Consciousness means the person is:
• Awake
• Aware of their surroundings
• Able to respond'''),
      const ReadingStep(
          id: 'R2', title: 'Normal vs Not Normal Consciousness', content: '''
What Is Normal?
• Responds when spoken to
• Answers questions clearly
• Can follow simple instructions

What Is Not Normal?
• Slow or confused response
• Cannot answer properly
• No response at all


Note:
Normal → No immediate danger
Not Normal → Something is wrong'''),
      MultiChoiceStep(
        id: 'Q1',
        instructions: "Choose the best answer.",
        title: "Person responds when spoken to and answers questions clearly.",
        choices: ["Normal", "Not Normal"],
        correctIndex: 0,
        correctExplanation:
            "Responding clearly when spoken to is a key sign of normal consciousness.",
        incorrectExplanation:
            "Clear and appropriate responses indicate normal consciousness.",
        hint: "Look for inability to speak or severe distress.",
        disclaimer:
            "Assume this is a sudden situation and the person was previously stable.",
        spriteAsset: 'assets/spritesheet/BreathingO.png',
      ),
      MultiChoiceStep(
        id: 'Q2',
        instructions: "Choose the best answer.",
        title: "Person responds but is slow and confused.",
        choices: [
          "Normal",
          "Not Normal",
        ],
        correctIndex: 1,
        correctExplanation:
            "A slow or confused response means the person is not in a normal state.",
        incorrectExplanation:
            "Normal consciousness requires clear and appropriate responses.",
        hint: "Normal responses should be clear and appropriate.",
        disclaimer:
            "Assume this is a sudden situation and the person was previously stable.",
        spriteAsset: 'assets/spritesheet/BreathingO.png',
      ),
      const TextInputStep(
          id: "Q3",
          instructions: "How would you describe this condition?",
          title: "Person does not respond when spoken to.",
          aiPrompt: 'Give a brief explanation of the answer',
          spriteAsset: 'assets/spritesheet/BreathingO.png',
          hint: "Is the person able to respond?"),
      const ReadingStep(id: ' R3', title: 'What Is Consciousness?', content: '''
Normal
• Responds clearly
• Answers questions correctly
• Follows instructions

Concerning
• Responds but confused or slow
• Answers incorrectly
• Not fully aware

Emergency
• No response at all


Note:
Concerning → Monitor closely
Emergency → Immediate action needed'''),
      MultiChoiceStep(
        id: 'Q4',
        instructions: "Choose the best answer.",
        title: "Person responds but seems confused and slow.",
        choices: [
          "Normal",
          "Concerning",
          "Emergency",
        ],
        correctIndex: 1,
        correctExplanation:
            "The person is still responding, but not clearly, which makes it concerning.",
        incorrectExplanation:
            "Since the person can still respond, it is not an emergency, but also not normal.",
        hint: "Is the person still responding?",
        disclaimer:
            "Assume this is a sudden situation and the person was previously stable.",
        spriteAsset: 'assets/spritesheet/BreathingO.png',
      ),
      MultiChoiceStep(
        id: 'Q5',
        instructions: "Choose the best answer.",
        title: "Person does not respond at all.",
        choices: [
          "Normal",
          "Not Normal",
          "Concerning",
          "Emergency",
        ],
        correctIndex: 3,
        correctExplanation:
            "No response at all means the person is in an emergency condition.",
        incorrectExplanation:
            "Lack of response indicates a life-threatening condition requiring immediate action.",
        hint: "What does no response indicate?",
        disclaimer:
            "Assume this is a sudden situation and the person was previously stable.",
        spriteAsset: 'assets/spritesheet/BreathingO.png',
      ),
      const TextInputStep(
          id: "Q6",
          instructions: "How serious is this condition?",
          title: "Person answers clearly and follows instructions.",
          aiPrompt: 'Give a brief explanation of the answer',
          spriteAsset: 'assets/spritesheet/BreathingO.png',
          hint: 'Check if the response is clear and correct.')
    ],
  ),
  /*

      LESSON 3

  */
  const LessonItem(
    title: 'Chest Pain and Heart Attack',
    spriteAsset: 'assets/spritesheet/ChestPain.png',
    steps: [
      ReadingStep(
        id: 'r1',
        title: 'Title',
        content: 'Content blah blah blah',
      ),
    ],
  ),
  /*

      LESSON 4

  */
  const LessonItem(
    title: 'Bleeding / Wound Management',
    spriteAsset: 'assets/spritesheet/Bleeding.png',
    steps: [
      ReadingStep(
        id: 'r1',
        title: 'Title',
        content: 'Content blah blah blah',
      ),
    ],
  ),
  /*

      LESSON 5

  */
  const LessonItem(
    title: 'Shock and Unconscious',
    spriteAsset: 'assets/spritesheet/Shocked.png',
    steps: [
      ReadingStep(
        id: 'r1',
        title: 'Title',
        content: 'Content blah blah blah',
      ),
    ],
  ),
];
