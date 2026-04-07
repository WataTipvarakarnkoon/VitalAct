import '../models/lesson_step.dart';
import '../models/steps/reading_step.dart';
import '../models/steps/multi_choice_step.dart';
import '../models/steps/text_input_step.dart';
import '../models/steps/order_question_step.dart';

class StepFactory {
  static LessonStep build(Map<String, dynamic> json) {
    switch (json['type']) {
      case 'reading':
        return ReadingStep(
          id: json['id'],
          title: json['title'],
          content: json['content'],
          type: 'reading',
        );

      case 'mcq':
        return MultiChoiceStep(
          id: json['id'],
          title: json['title'],
          instructions: json['instructions'],
          choices: List<String>.from(json['choices']),
          correctIndex: json['correctIndex'],
          correctExplanation: json['correctExplanation'],
          incorrectExplanation: json['incorrectExplanation'],
          hint: json['hint'],
          disclaimer: json['disclaimer'] ?? '',
          spriteAsset: json['spriteAsset'],
          type: 'mcq',
        );

      case 'text':
        return TextInputStep(
          id: json['id'],
          title: json['title'],
          instructions: json['instructions'],
          aiPrompt: json['aiPrompt'],
          hint: json['hint'],
          spriteAsset: json['spriteAsset'],
          type: 'text',
        );

      case 'order':
        return OrderQuestionStep(
          id: json['id'],
          title: json['title'],
          instructions: json['instructions'],
          choices: List<String>.from(json['choices']),
          correctOrder: List<int>.from(json['correctOrder']),
          correctExplanation: json['correctExplanation'],
          incorrectExplanation: json['incorrectExplanation'],
          hint: json['hint'],
          spriteAsset: json['spriteAsset'],
          type: 'order',
        );

      default:
        throw Exception('Unknown step type: ${json['type']}');
    }
  }
}
