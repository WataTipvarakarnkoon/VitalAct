import '../lesson_step.dart';

class OrderQuestionStep extends LessonStep {
  final String instructions;
  final String title;
  final List<String> choices;
  final List<int> correctOrder;
  final String correctExplanation;
  final String incorrectExplanation;
  final String hint;
  final String spriteAsset;

  OrderQuestionStep({
    required super.id,
    required this.instructions,
    required this.title,
    required this.choices,
    required this.correctOrder,
    required this.correctExplanation,
    required this.incorrectExplanation,
    required this.hint,
    required this.spriteAsset,
  });
}
