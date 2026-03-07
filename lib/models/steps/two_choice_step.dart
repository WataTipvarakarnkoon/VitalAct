import '../lesson_step.dart';

class TwoChoiceStep extends LessonStep {
  final String instructions;
  final String title;
  final String imageAsset;
  final String disclaimer;

  final List<String> choices;

  TwoChoiceStep({
    required this.instructions,
    required this.title,
    required this.imageAsset,
    required this.disclaimer,
    required this.choices,
    required super.id,
  });
}
