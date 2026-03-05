import 'package:flutter/material.dart';

class LessonItem {
  final String title;
  final String description;
  final Widget page;

  const LessonItem({
    required this.title,
    required this.description,
    required this.page,
  });
}
