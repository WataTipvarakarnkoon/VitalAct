import 'dart:math';
import 'package:vitalact/models/lesson_item.dart';

class PickQuestion {
  final LessonItem lessonType;
  final dynamic qs;

  PickQuestion({required this.lessonType, required this.qs});
}

List<PickQuestion> pickRandomQuestions(List<LessonItem> lessonData) {
  final random = Random();
  List<PickQuestion> all = [];

  for (LessonItem lesson in lessonData) {
    List<dynamic> questions = lesson.steps.where((stepQ) {
      return stepQ.id.trim().startsWith('Q');
    }).toList();

    questions.shuffle(random);

    for (var steps in questions.take(2)) {
      all.add(PickQuestion(lessonType: lesson, qs: steps));
    }
  }

  all.shuffle(random);
  return all;
}
