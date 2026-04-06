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
      ReadingStep(
        id: 'L1_R1',
        title: 'What is First Aid?',
        content: '''
First aid is the immediate help given before professionals arrive.

Goals:
• Save life
• Prevent worsening
• Promote recovery''',
        type: 'reading',
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
        type: 'multi_choice',
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
        type: 'multi_choice',
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
        type: 'order',
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
        type: 'multi_choice',
      ),
      TextInputStep(
        id: 'L1_Q5',
        title: 'Why must first aid be immediate?',
        instructions: 'Explain briefly.',
        aiPrompt: 'Give a short explanation',
        spriteAsset: 'assets/spritesheet/NVSA.png',
        hint: 'Think about delays and risk.',
        type: 'text_input',
      ),
    ],
  ),
];
