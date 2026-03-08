import 'package:flutter/material.dart';
import 'package:vitalact/models/steps/reading_step.dart';
import 'package:vitalact/models/steps/two_choice_step.dart';
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
    title: 'Normal vs abnormal signs',
    description:
        'Learn to quickly recognize abnormal breathing and emergency warning signs.',
    steps: [
      const ReadingStep(
        id: 'What-is-normal',
        title: 'What Is Normal?',
        content:
            'Normal Breathing\n • 12–20 breaths per minute (adult)\n • Regular rhythm\n • Can speak full sentences\n • Skin normal color\n\n\nNote:\nNormal → No immediate danger\nConcerning → Monitor closely\nEmergency → Immediate action needed',
      ),
      TwoChoiceStep(
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
      )
    ],
  ),
  const LessonItem(
    title: 'Life-threatening red flags',
    description:
        'Identify critical signs that require immediate emergency action.',
    steps: [
      ReadingStep(
        id: 'r1',
        title: 'Title',
        content: 'Content blah blah blah',
      ),
    ],
  ),
  const LessonItem(
    title: 'Breathing Distress Detection',
    description: 'Recognize early and severe signs of breathing difficulty.',
    steps: [
      ReadingStep(
        id: 'r1',
        title: 'Title',
        content: 'Content blah blah blah',
      ),
    ],
  ),
  const LessonItem(
    title: 'Chest Pain & Stroke Clues',
    description: 'Spot key symptoms of heart attack and stroke quickly.',
    steps: [
      ReadingStep(
        id: 'r1',
        title: 'Title',
        content: 'Content blah blah blah',
      ),
    ],
  ),
  const LessonItem(
    title: 'Mixed Symptom Recognition Challenge',
    description:
        'Practice identifying emergencies when multiple symptoms appear.',
    steps: [
      ReadingStep(
        id: 'r1',
        title: 'Title',
        content: 'Content blah blah blah',
      ),
    ],
  ),
];
