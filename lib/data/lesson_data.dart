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
  LessonItem(
    title: 'Normal VS Abnormal signs',
    spriteAsset: 'assets/spritesheet/NVSA.png',
    steps: [
      const ReadingStep(
        id: 'What-is-normal',
        title: 'What Is Normal?',
        content: '''
Normal Breathing
 • 12–20 breaths per minute (adult)
 • Regular rhythm
 • Can speak full sentences
 • Skin normal color


Note:
Normal → No immediate danger
Concerning → Monitor closely
Emergency → Immediate action needed''',
      ),
      MultiChoiceStep(
        id: 'Q1',
        title: 'Person breathing 22 times per minute but speaking clearly.',
        instructions: 'Choose the best answer.',
        imageAsset: 'assets/images/test_lesson_image.png',
        choices: const ['Normal', 'Emergency'],
        correctIndex: 1,
        disclaimer:
            'Assume this is a sudden situation and the person was previously stable.',
        correctExplanation:
            "22 breaths per minute is above the normal adult range (12–20) and may signal early distress.",
        incorrectExplanation:
            "Even if the person can speak clearly, 22 breaths per minute is faster than normal.",
        hint: "Compare the breathing rate to the normal adult range (12–20).",
      ),
      MultiChoiceStep(
        id: 'Q2',
        title:
            'Person breathing 18/min but cannot finish sentences without pausing.',
        instructions: 'Choose the best answer.',
        imageAsset: 'assets/images/test_lesson_image.png',
        choices: const ['Normal', 'Emergency'],
        correctIndex: 1,
        disclaimer:
            'Assume this is a sudden situation and the person was previously stable.',
        correctExplanation:
            "Difficulty speaking full sentences suggests breathing distress even if the rate is normal.",
        incorrectExplanation:
            "A normal breathing rate does not rule out an emergency if the person cannot speak normally.",
        hint:
            "Focus on the person's ability to speak, not just the breathing rate.",
      ),
      MultiChoiceStep(
        id: 'Q3',
        title:
            'Person breathing 10/min, awake, answering slowly but correctly.',
        instructions: 'Choose the best answer.',
        imageAsset: 'assets/images/test_lesson_image.png',
        choices: const ['Normal', 'Concerning', 'Emergency'],
        correctIndex: 1,
        disclaimer:
            'Assume this is a sudden situation and the person was previously stable.',
        correctExplanation:
            "10 breaths per minute is slower than the normal adult range (12–20).",
        incorrectExplanation:
            "Breathing slower than the normal range (12–20) should not be considered normal.",
        hint: "Check whether the breathing rate is below the normal range.",
      ),
      const TextInputStep(
          id: 'Q4',
          instructions: 'instructions',
          title: 'title',
          imageAsset: 'assets/images/test_lesson_image.png',
          correctAnswers: ['correctAnswers'],
          correctExplanation: 'correctExplanation',
          incorrectExplanation: 'incorrectExplanation',
          hint: 'hint')
    ],
  ),
  const LessonItem(
    title: 'Life-threatening Red flags',
    spriteAsset: 'assets/spritesheet/RedFlags.png',
    steps: [
      ReadingStep(
        id: 'r1',
        title: 'Title',
        content: 'Content blah blah blah',
      ),
    ],
  ),
  const LessonItem(
    title: 'Breathing distress Detection',
    steps: [
      ReadingStep(
        id: 'r1',
        title: 'Title',
        content: 'Content blah blah blah',
      ),
    ],
  ),
  const LessonItem(
    title: 'Chest pain & Stroke clues',
    steps: [
      ReadingStep(
        id: 'r1',
        title: 'Title',
        content: 'Content blah blah blah',
      ),
    ],
  ),
  const LessonItem(
    title: 'Mixed symptom Recognition Challenge',
    steps: [
      ReadingStep(
        id: 'r1',
        title: 'Title',
        content: 'Content blah blah blah',
      ),
    ],
  ),
];
