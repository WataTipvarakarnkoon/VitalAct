import 'package:flutter/material.dart';
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
    page: PlaceholderPage(title: 'Normal vs abnormal signs'),
  ),
  const LessonItem(
    title: 'Life-threatening red flags',
    description:
        'Identify critical signs that require immediate emergency action.',
    page: PlaceholderPage(title: 'Life-threatening red flags'),
  ),
  const LessonItem(
    title: 'Breathing Distress Detection',
    description: 'Recognize early and severe signs of breathing difficulty.',
    page: PlaceholderPage(title: 'Breathing Distress Detection'),
  ),
  const LessonItem(
    title: 'Chest Pain & Stroke Clues',
    description: 'Spot key symptoms of heart attack and stroke quickly.',
    page: PlaceholderPage(title: 'Chest Pain & Stroke Clues'),
  ),
  const LessonItem(
    title: 'Mixed Symptom Recognition Challenge',
    description:
        'Practice identifying emergencies when multiple symptoms appear.',
    page: PlaceholderPage(title: 'Mixed Symptom Recognition Challenge'),
  ),
];
