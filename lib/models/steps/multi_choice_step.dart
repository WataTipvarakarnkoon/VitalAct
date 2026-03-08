import '../lesson_step.dart';

class MultiChoiceStep extends LessonStep {
  final String instructions;
  final String title;
  final String imageAsset;
  final String disclaimer;
  final int correctIndex;

  final List<String> choices;

  /// Feedback content
  final String correctExplanation;
  final String incorrectExplanation;
  final String? hint;

  MultiChoiceStep({
    required this.instructions,
    required this.title,
    required this.imageAsset,
    required this.disclaimer,
    required this.correctIndex,
    required this.choices,
    required this.correctExplanation,
    required this.incorrectExplanation,
    this.hint,
    required super.id,
  });
}
