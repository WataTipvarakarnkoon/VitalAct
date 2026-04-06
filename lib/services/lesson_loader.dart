import 'dart:convert';
import 'package:flutter/services.dart';
import 'package:vitalact/models/lesson_item.dart';
import 'package:vitalact/services/step_factory.dart';

class LessonLoader {
  static Future<List<LessonItem>> loadLessons() async {
    final String jsonString =
        await rootBundle.loadString('assets/data/lessons.json');

    final List<dynamic> data = json.decode(jsonString);

    return data.map((lessonJson) {
      return LessonItem(
        id: lessonJson['id'],
        title: lessonJson['title'],
        spriteAsset: lessonJson['spriteAsset'],
        steps: (lessonJson['steps'] as List)
            .map((stepJson) => StepFactory.build(stepJson))
            .toList(),
      );
    }).toList();
  }
}
