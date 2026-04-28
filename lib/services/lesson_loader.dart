import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:vitalact/models/lesson_module.dart';
import 'package:vitalact/models/lesson_item.dart';
import 'package:vitalact/services/step_factory.dart';

class LessonLoader {
  static const _moduleFiles = [
    'module1.json',
    'module2.json',
  ];

  static Future<List<LessonModule>> loadAllModules([String locale = 'en']) async {
    List<LessonModule> modules = [];

    for (final file in _moduleFiles) {
      final module = await _loadModule(file, locale);
      if (module != null) modules.add(module);
    }

    return modules;
  }

  static Future<LessonModule?> _loadModule(String file, String locale) async {
    final localePath = 'assets/data/$locale/$file';
    final fallbackPath = 'assets/data/$file';

    String? jsonString;

    try {
      jsonString = await rootBundle.loadString(localePath);
    } catch (_) {
      try {
        jsonString = await rootBundle.loadString(fallbackPath);
      } catch (e) {
        debugPrint('ERROR loading $file: $e');
        return null;
      }
    }

    try {
      final Map<String, dynamic> data = json.decode(jsonString);
      return LessonModule(
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
    } catch (e) {
      debugPrint('ERROR parsing $file: $e');
      return null;
    }
  }
}
