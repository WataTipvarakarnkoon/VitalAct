import 'package:flutter/material.dart';
import 'package:vitalact/models/steps/reading_step.dart';
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
  const LessonItem(
    title: 'Normal vs abnormal signs',
    description:
        'Learn to quickly recognize abnormal breathing and emergency warning signs.',
    steps: [
      ReadingStep(
        id: 'r1',
        title: 'What Is Normal?',
        content:
            'Normal Breathing\n • 12–20 breaths per minute (adult)\n • Regular rhythm\n • Can speak full sentences\n • Skin normal color\n\n\nNote:\nNormal → No immediate danger\nConcerning → Monitor closely\nEmergency → Immediate action needed',
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
