import 'dart:convert';
import 'package:flutter/services.dart';
import '../models/lesson_item.dart';

class LessonLoader {
  static Future<List<LessonItem>> loadModule(String path) async {
    final jsonString = await rootBundle.loadString(path);
    final data = json.decode(jsonString);

    return (data as List).map((lesson) => LessonItem.fromJson(lesson)).toList();
  }
}
