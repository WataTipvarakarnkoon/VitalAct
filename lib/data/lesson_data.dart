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

      LESSON 1 Breathing

  */
  LessonItem(
    title: 'Breathing Assessment',
    spriteAsset: 'assets/spritesheet/NVSA.png',
    steps: [
      const ReadingStep(
        id: 'R1',
        title: 'Breathing Assessment',
        content: '''
Normal
• Breathing regular and effortless
• Can speak full sentences
• No visible distress

Not Normal
• Breathing slightly irregular or labored
• Pauses while speaking
• Mild anxiety or restlessness

Note:
Normal → No immediate danger
Not Normal → Something is wrong''',
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
      const ReadingStep(
        id: 'R2',
        title: 'Urgency Assessment',
        content: '''
Concerning
• Breathing fast or slow beyond normal range
• Struggling to speak
• Noticeable chest movement or effort

Emergency
• No breathing or gasping irregularly
• Cannot speak or respond properly
• Cyanosis (bluish lips or fingers)
• Collapse or fainting

Note:
Concerning → Monitor closely
Emergency → Immediate action needed''',
      ),
      MultiChoiceStep(
        id: 'Q3',
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
      MultiChoiceStep(
        id: 'Q4',
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

      LESSON 2 Consciousness

  */
  LessonItem(
    title: 'Consciousness Check',
    spriteAsset: 'assets/spritesheet/RedFlags.png',
    steps: [
      const ReadingStep(id: ' R1', title: 'What Is Consciousness?', content: '''
Consciousness means:
• Awake
• Aware of their surroundings
• Able to respond'''),
      const ReadingStep(id: 'R2', title: 'Basic Observation', content: '''
Normal
• Responds clearly
• Answers questions correctly
• Follows instructions

Not Normal
• Slow or confused response
• Cannot answer properly
• No response at all


Note:
Normal → No immediate danger
Not Normal → Something is wrong'''),
      MultiChoiceStep(
        id: 'Q1',
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
      MultiChoiceStep(
        id: 'Q2',
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
      const TextInputStep(
          id: "Q3",
          instructions: "How would you describe this condition?",
          title: "Person does not respond when spoken to.",
          aiPrompt: 'Give a brief explanation of the answer',
          spriteAsset: 'assets/spritesheet/BreathingO.png',
          hint: "Is the person able to respond?"),
      const ReadingStep(id: ' R3', title: 'Urgency Assessment', content: '''
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

      LESSON 3 Chest Pain

  */
  LessonItem(
    title: 'Chest Pain and Heart Attack',
    spriteAsset: 'assets/spritesheet/ChestPain.png',
    steps: [
      const ReadingStep(
        id: 'R1',
        title: 'Observation Signs',
        content: '''
Normal
• Calm and relaxed
• No sweating or pallor
• Moves normally

Not Normal
• Anxious or uneasy
• Mild facial grimacing or clutching chest briefly
• Slight sweating or nausea observable
• Shifts position frequently


Note:
Normal → No immediate danger
Not → Normal Something is wrong''',
      ),
      MultiChoiceStep(
        id: 'Q1',
        instructions: "Choose the best answer.",
        title:
            "Person sitting calmly, relaxed posture, skin color normal, no sweating.",
        choices: [
          "Normal",
          "Not Normal",
        ],
        correctIndex: 0,
        correctExplanation:
            "Calm appearance with no signs of distress indicates stability.",
        incorrectExplanation:
            "Any signs of distress are absent, so immediate concern is not required.",
        hint: "Look for any observable abnormal signs.",
        disclaimer:
            "Assume this is a sudden situation and the person was previously stable.",
        spriteAsset: 'assets/spritesheet/BreathingO.png',
      ),
      MultiChoiceStep(
        id: 'Q2',
        instructions: "Choose the best answer.",
        title:
            "Person shifting position frequently, mild facial grimacing, slightly sweaty.",
        choices: [
          "Normal",
          "Not Normal",
        ],
        correctIndex: 1,
        correctExplanation:
            "Observable anxiety or mild discomfort suggests the person may be at risk.",
        incorrectExplanation:
            "Lack of severe distress means life-threatening risk is low.",
        hint: "Are there any visible indications of discomfort?",
        disclaimer:
            "Assume this is a sudden situation and the person was previously stable.",
        spriteAsset: 'assets/spritesheet/BreathingO.png',
      ),
      const ReadingStep(
        id: 'R2',
        title: 'Urgency Assessment',
        content: '''
Concerning
• Mild chest discomfort
• Anxious or uneasy
• Slight sweating or nausea
• Shifts position frequently

Emergency
• Repeatedly clutching chest
• Pain gestures toward arm, neck, jaw, or back
• Pale, sweaty, or nauseous
• Fainting or collapse


Note:
Concerning – Monitor closely and be ready to assist
Emergency – Call emergency services immediately and prepare to act''',
      ),
      MultiChoiceStep(
        id: 'Q3',
        instructions: "Choose the best answer.",
        title:
            "Person clutching chest repeatedly, leaning forward, pale and sweaty.",
        choices: [
          "Normal",
          "Concerning",
          "Emergency",
        ],
        correctIndex: 2,
        correctExplanation:
            "Sudden severe chest distress with pallor and sweating indicates a life-threatening condition requiring immediate action.",
        incorrectExplanation:
            "Mild or absent visual signs indicate the condition is not immediately life-threatening.",
        hint: "Look for severe observable distress or collapse.",
        disclaimer:
            "Assume this is a sudden situation and the person was previously stable.",
        spriteAsset: 'assets/spritesheet/BreathingO.png',
      ),
      MultiChoiceStep(
        id: 'Q4',
        instructions: "Choose the best answer.",
        title:
            "Person rubbing their chest occasionally, mildly anxious, slightly sweaty but still able to speak normally.",
        choices: [
          "Normal",
          "Concerning",
          "Emergency",
        ],
        correctIndex: 1,
        correctExplanation:
            "Mild chest discomfort with slight sweating are early warning signs — not yet an emergency but worth monitoring.",
        incorrectExplanation:
            "Chest rubbing and sweating are too mild for emergency, but not normal either.",
        hint: "Look for mild discomfort signs, not calm nor collapsing.",
        disclaimer:
            "Assume this is a sudden situation and the person was previously stable.",
        spriteAsset: 'assets/spritesheet/BreathingO.png',
      ),
      const TextInputStep(
          id: "Q6",
          instructions:
              "List three observable signs indicating a possible heart attack.",
          title:
              "Person suddenly collapses, clutching chest, pale, sweating, leaning forward.",
          aiPrompt: 'Give a brief explanation of the answer',
          spriteAsset: 'assets/spritesheet/BreathingO.png',
          hint: 'Focus on visual signs only.')
    ],
  ),
  /*

      LESSON 4 Bleeding

  */
  LessonItem(
    title: 'Bleeding / Wound Management',
    spriteAsset: 'assets/spritesheet/Bleeding.png',
    steps: [
      const ReadingStep(
        id: 'R1',
        title: 'What is bleeding?',
        content: '''
Bleeding happens when blood flows out of the body due to a wound.

It can be:
• Minor (small cut)
• Moderate
• Severe (heavy bleeding)

Check:
• Amount of blood
• Speed of bleeding
• Where the bleeding is''',
      ),
      const ReadingStep(id: 'R2', title: 'Basic Observation', content: '''
Normal
• No bleeding
• Skin is intact (no open wound)

Not Normal
• Any visible bleeding
• Open wound or cut
• Blood coming out of the body


Note:
Normal → No immediate danger
Not Normal → Something is wrong'''),
      MultiChoiceStep(
        id: 'Q1',
        instructions: "Choose the best answer.",
        title: "Small cut with slow bleeding.",
        choices: [
          "Normal",
          "Not Normal",
        ],
        correctIndex: 1,
        correctExplanation:
            "Even small bleeding means the condition is not normal.",
        incorrectExplanation: "Normal means no bleeding at all.",
        hint: "Any bleeding is not fully normal",
        disclaimer:
            "Assume this is a sudden situation and the person was previously stable.",
        spriteAsset: 'assets/spritesheet/BreathingO.png',
      ),
      MultiChoiceStep(
        id: 'Q2',
        instructions: "Choose the best answer.",
        title: "No bleeding is seen.",
        choices: [
          "Normal",
          "Not Normal",
        ],
        correctIndex: 0,
        correctExplanation:
            "No bleeding means there is no injury involving blood.",
        incorrectExplanation: "Without bleeding, the situation is normal.",
        hint: "Is there any blood?",
        disclaimer:
            "Assume this is a sudden situation and the person was previously stable.",
        spriteAsset: 'assets/spritesheet/BreathingO.png',
      ),
      const TextInputStep(
          id: "Q3",
          instructions: "Describe the condition.",
          title: "Small wound with slow bleeding that stops easily.",
          aiPrompt: 'Give a brief explanation of the answer',
          spriteAsset: 'assets/spritesheet/BreathingO.png',
          hint: 'Is it severe or mild?'),
      const ReadingStep(id: 'R3', title: 'Basic Observation', content: '''
Concerning
• Small or slow bleeding
• Blood is controlled

Emergency
• Heavy bleeding
• Blood flowing continuously
• Cannot be controlled

Note:
Concerning → Monitor closely
Emergency → Immediate action needed'''),
      MultiChoiceStep(
        id: 'Q4',
        instructions: "Choose the best answer.",
        title: "Bleeding steadily from a cut but not heavy.",
        choices: [
          "Normal",
          "Concerning",
          "Emergency",
        ],
        correctIndex: 1,
        correctExplanation:
            "Moderate bleeding that is not severe is concerning but not an emergency.",
        incorrectExplanation:
            "It is not normal, but also not severe enough to be an emergency.",
        hint: "Check the amount and speed",
        disclaimer:
            "Assume this is a sudden situation and the person was previously stable.",
        spriteAsset: 'assets/spritesheet/BreathingO.png',
      ),
      MultiChoiceStep(
        id: 'Q4',
        instructions: "Choose the best answer.",
        title: "Heavy bleeding that does not stop.",
        choices: [
          "Normal",
          "Not Normal"
              "Concerning",
          "Emergency",
        ],
        correctIndex: 3,
        correctExplanation:
            "Uncontrolled heavy bleeding is life-threatening and requires immediate action.",
        incorrectExplanation: "Severe, uncontrolled bleeding is an emergency.",
        hint: "Can the bleeding be controlled?",
        disclaimer:
            "Assume this is a sudden situation and the person was previously stable.",
        spriteAsset: 'assets/spritesheet/BreathingO.png',
      ),
      const TextInputStep(
          id: "Q6",
          instructions: "How urgent is this situation?",
          title: "Blood is flowing quickly from a wound and does not stop.",
          aiPrompt: 'Give a brief explanation of the answer',
          spriteAsset: 'assets/spritesheet/BreathingO.png',
          hint: 'Check the speed and whether the bleeding stops.'),
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
