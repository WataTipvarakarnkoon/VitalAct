import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:vitalact/models/lesson_module.dart';
import 'package:vitalact/models/lesson_item.dart';
import 'package:vitalact/services/step_factory.dart';

class LessonLoader {
  static Future<List<LessonModule>> loadAllModules() async {
    final paths = [
      'assets/data/module1.json',
      'assets/data/module2.json',
    ];

    List<LessonModule> modules = [];

    for (final path in paths) {
      try {
        final String jsonString = await rootBundle.loadString(path);
        final Map<String, dynamic> data = json.decode(jsonString);

        final module = LessonModule(
          moduleTitle: data['title'] ?? data['moduleTitle'] ?? 'Unknown',
          lessons: (data['lessons'] as List).map((lessonJson) {
            return LessonItem(
              id: lessonJson['id'],
              title: lessonJson['title'],
              spriteAsset: lessonJson['spriteAsset'] ?? '',
              steps: (lessonJson['steps'] as List)
                  .map((stepJson) => StepFactory.build(stepJson))
                  .toList(),
            );
          }).toList(),
        );

        modules.add(module);
      } catch (e) {
        debugPrint('🔥 ERROR loading $path: $e');
      }
    }

    return modules;
  }
}
