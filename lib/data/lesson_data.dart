import 'package:flutter/material.dart';
import 'package:vitalact/models/steps/reading_step.dart';
import 'package:vitalact/models/steps/multi_choice_step.dart';
import 'package:vitalact/models/steps/text_input_step.dart';
import '../models/lesson_item.dart';
import 'package:vitalact/models/steps/order_question_step.dart';

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

const String sprite = 'assets/spritesheet/NVSA.png';

final List<LessonItem> lessonData = [
/* =========================================================
   LESSON 1: First Aid Basics
========================================================= */

  LessonItem(
    id: 'L1',
    title: 'First Aid Basics',
    spriteAsset: sprite,
    steps: [
      const ReadingStep(
        id: 'L1_R1',
        title: 'What is First Aid?',
        content: '''
First aid is the immediate help given before professionals arrive.

Goals:
• Save life
• Prevent worsening
• Promote recovery''',
      ),
      MultiChoiceStep(
        id: 'L1_Q1',
        title: 'What is the main purpose of first aid?',
        instructions: 'Choose the best answer.',
        spriteAsset: sprite,
        choices: const [
          'Replace doctors',
          'Provide immediate help',
          'Perform surgery',
        ],
        correctIndex: 1,
        disclaimer: '',
        correctExplanation:
            'First aid provides immediate care before professionals arrive.',
        incorrectExplanation:
            'First aid is not meant to replace doctors or perform surgery.',
        hint: 'Think about timing.',
      ),
      MultiChoiceStep(
        id: 'L1_Q2',
        title: 'Which is NOT a goal of first aid?',
        instructions: 'Choose the best answer.',
        spriteAsset: sprite,
        choices: const [
          'Save life',
          'Prevent worsening',
          'Perform surgery',
        ],
        correctIndex: 2,
        disclaimer: '',
        correctExplanation: 'Surgery is not part of first aid.',
        incorrectExplanation:
            'Saving life and preventing worsening are core goals.',
        hint: 'Basic vs advanced care.',
      ),
      OrderQuestionStep(
        id: 'L1_Q3',
        title: 'Arrange first aid priorities',
        instructions: 'Put in correct order.',
        spriteAsset: sprite,
        choices: const [
          'Promote recovery',
          'Save life',
          'Prevent worsening',
        ],
        correctOrder: const [1, 2, 0],
        correctExplanation:
            'Correct order: save life → prevent worsening → promote recovery.',
        incorrectExplanation: 'Life-threatening issues must always come first.',
        hint: 'What is most urgent?',
      ),
      MultiChoiceStep(
        id: 'L1_Q4',
        title: 'Why is early recognition important?',
        instructions: 'Choose the best answer.',
        spriteAsset: sprite,
        choices: const [
          'It helps you act faster',
          'It replaces treatment',
          'It is optional',
        ],
        correctIndex: 0,
        disclaimer: '',
        correctExplanation:
            'Recognizing problems early allows faster and correct action.',
        incorrectExplanation: 'Recognition is essential, not optional.',
        hint: 'Think speed.',
      ),
      const TextInputStep(
        id: 'L1_Q5',
        title: 'Why must first aid be immediate?',
        instructions: 'Explain briefly.',
        aiPrompt: 'Give a short explanation',
        spriteAsset: 'assets/spritesheet/NVSA.png',
        hint: 'Think about delays and risk.',
      ),
    ],
  ),

/* =========================================================
   LESSON 2: Scene Safety
========================================================= */

  LessonItem(
    id: 'L2',
    title: 'Scene Safety',
    spriteAsset: sprite,
    steps: [
      const ReadingStep(
        id: 'L2_R1',
        title: 'Stay Safe First',
        content: '''
Before helping, always check the scene.

Possible dangers:
• Fire
• Traffic
• Electricity

Rule:
Do not become the second victim.''',
      ),
      MultiChoiceStep(
        id: 'L2_Q1',
        title: 'What should you check first?',
        instructions: 'Choose the best answer.',
        spriteAsset: sprite,
        choices: const [
          'Victim condition',
          'Scene safety',
          'Call for help',
        ],
        correctIndex: 1,
        disclaimer: '',
        correctExplanation: 'Your safety always comes first.',
        incorrectExplanation:
            'You must ensure the scene is safe before helping.',
        hint: 'Think about your own safety.',
      ),
      MultiChoiceStep(
        id: 'L2_Q2',
        title: 'Why is scene safety important?',
        instructions: 'Choose the best answer.',
        spriteAsset: sprite,
        choices: const [
          'To avoid injury',
          'To delay action',
          'To observe longer',
        ],
        correctIndex: 0,
        disclaimer: '',
        correctExplanation: 'You cannot help others if you are injured.',
        incorrectExplanation: 'Safety ensures you can continue helping.',
        hint: 'Self-protection first.',
      ),
      OrderQuestionStep(
        id: 'L2_Q3',
        title: 'Arrange safety steps',
        instructions: 'Put in correct order.',
        spriteAsset: sprite,
        choices: const [
          'Help the victim',
          'Check surroundings',
          'Ensure personal safety',
        ],
        correctOrder: const [1, 2, 0],
        correctExplanation: 'Check surroundings → ensure safety → help victim.',
        incorrectExplanation: 'Helping without checking can be dangerous.',
        hint: 'Safety before action.',
      ),
      MultiChoiceStep(
        id: 'L2_Q4',
        title: 'You see danger nearby. What do you do?',
        instructions: 'Choose the best answer.',
        spriteAsset: sprite,
        choices: const [
          'Rush immediately',
          'Assess the situation',
          'Ignore the danger',
        ],
        correctIndex: 1,
        disclaimer: '',
        correctExplanation: 'You must assess before acting to avoid harm.',
        incorrectExplanation:
            'Rushing or ignoring danger can make things worse.',
        hint: 'Pause and check.',
      ),
      const TextInputStep(
        id: 'L2_Q5',
        title: 'Give one example of a hazard.',
        instructions: 'Short answer.',
        aiPrompt: 'Give one example of a dangerous situation',
        spriteAsset: 'assets/spritesheet/NVSA.png',
        hint: 'Think environmental risks.',
      ),
    ],
  ),

/* =========================================================
   LESSON 3: First Response Flow
========================================================= */

  LessonItem(
    id: 'L3',
    title: 'First Response Flow',
    spriteAsset: sprite,
    steps: [
      const ReadingStep(
        id: 'L3_R1',
        title: 'Basic Response Steps',
        content: '''
Follow this order:
1. Check safety
2. Check the person
3. Call for help
4. Take action''',
      ),
      OrderQuestionStep(
        id: 'L3_Q1',
        title: 'Arrange the response steps',
        instructions: 'Put in correct order.',
        spriteAsset: sprite,
        choices: const [
          'Call for help',
          'Check safety',
          'Take action',
          'Check the person',
        ],
        correctOrder: const [1, 3, 0, 2],
        correctExplanation: 'Correct order: safety → person → call → action.',
        incorrectExplanation: 'Following the correct order prevents mistakes.',
        hint: 'Start with safety.',
      ),
      MultiChoiceStep(
        id: 'L3_Q2',
        title: 'When should you call for help?',
        instructions: 'Choose the best answer.',
        spriteAsset: sprite,
        choices: const [
          'Before checking anything',
          'After checking the person',
          'At the very end only',
        ],
        correctIndex: 1,
        disclaimer: '',
        correctExplanation:
            'You need to understand the situation before calling.',
        incorrectExplanation:
            'Calling too early or too late can be ineffective.',
        hint: 'Assess first.',
      ),
      MultiChoiceStep(
        id: 'L3_Q3',
        title: 'Why follow a sequence?',
        instructions: 'Choose the best answer.',
        spriteAsset: sprite,
        choices: const [
          'Avoid mistakes',
          'Look professional',
          'Save time randomly',
        ],
        correctIndex: 0,
        disclaimer: '',
        correctExplanation: 'A proper sequence reduces errors and confusion.',
        incorrectExplanation: 'Sequence ensures safe and correct actions.',
        hint: 'Think safety.',
      ),
      MultiChoiceStep(
        id: 'L3_Q4',
        title: 'What happens if you skip safety?',
        instructions: 'Choose the best answer.',
        spriteAsset: sprite,
        choices: const [
          'Nothing',
          'You may get injured',
          'You act faster',
        ],
        correctIndex: 1,
        disclaimer: '',
        correctExplanation: 'Ignoring safety can put you in danger.',
        incorrectExplanation: 'Safety is always critical.',
        hint: 'Risk matters.',
      ),
      const TextInputStep(
        id: 'L3_Q5',
        title: 'Why is order important?',
        instructions: 'Explain briefly.',
        aiPrompt: 'Short explanation',
        spriteAsset: 'assets/spritesheet/NVSA.png',
        hint: 'Think mistakes and safety.',
      ),
    ],
  ),
];
