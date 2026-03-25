import 'package:firebase_ai/firebase_ai.dart';
import 'package:flutter/material.dart';
import 'package:vitalact/widgets/app_text_field.dart';
import '../../../models/steps/text_input_step.dart';
import '../../../services/lesson_progress_service.dart';
import '../../../widgets/lesson/lesson_step_scaffold.dart';

class TextInputPage extends StatefulWidget {
  final TextInputStep step;
  final VoidCallback onNext;

  const TextInputPage({
    super.key,
    required this.step,
    required this.onNext,
  });

  @override
  State<TextInputPage> createState() => _TextInputPageState();
}

class _TextInputPageState extends State<TextInputPage> {
  int? aiScore;
  bool isAnalyzing = false;
  String? aiHint;
  final FocusNode focusNode = FocusNode();
  bool isTyping = false;
  final TextEditingController controller = TextEditingController();
  final model =
      FirebaseAI.googleAI().generativeModel(model: 'gemini-2.5-flash-lite');

  String? aiExplanation;
  bool? isCorrect;
  bool answered = false;

  Future<void> evaluateAnswer(String userAnswer) async {
    final prompt = [
      Content.text("""
You are evaluating a first aid training answer.

Scenario:
${widget.step.title}

User answer:
$userAnswer

Instructions:
${widget.step.aiPrompt}

Respond ONLY in this format:

SCORE: number from 1-10
CORRECT: true or false
EXPLANATION: brief explanation of the answer.
IMPROVEMENT: one short sentence about what could be improved in the answer.
HINT: short hint to guide the user toward the correct answer.

Scoring rules:
10 = perfect answer
7-9 = good answer but missing detail
4-6 = partially correct
1-3 = incorrect or unsafe

If the answer is correct:
- HINT should be "none"
- IMPROVEMENT should suggest how the answer could be more complete.
""")
    ];

    final response = model.generateContentStream(prompt);

    String fullText = "";

    await for (final chunk in response) {
      fullText += chunk.text ?? "";
    }

    bool aiCorrect = fullText.toLowerCase().contains("correct: true");

    String explanation = RegExp(r'EXPLANATION:\s*(.*)', caseSensitive: false)
            .firstMatch(fullText)
            ?.group(1)
            ?.trim() ??
        "";

    String hint = RegExp(r'HINT:\s*(.*)', caseSensitive: false)
            .firstMatch(fullText)
            ?.group(1)
            ?.trim() ??
        "";
    String improvement = RegExp(r'IMPROVEMENT:\s*(.*)', caseSensitive: false)
            .firstMatch(fullText)
            ?.group(1)
            ?.trim() ??
        "";
    int score = int.tryParse(RegExp(r'SCORE:\s*(\d+)', caseSensitive: false)
                .firstMatch(fullText)
                ?.group(1) ??
            "0") ??
        0;

    if (!mounted) return;
    setState(() {
      isAnalyzing = false;
      isCorrect = aiCorrect;
      aiScore = score;

      aiExplanation =
          "Score: $score / 10\n\n$explanation${improvement.isNotEmpty ? "\n\nWhat could be improved:\n$improvement" : ""}";

      aiHint = (!aiCorrect && hint != "none") ? hint : null;
    });

    LessonProgressService.recordAnswer(aiCorrect, score);
  }

  void submit() {
    if (answered) return;

    final input = controller.text.trim().toLowerCase();
    final correct =
        widget.step.correctAnswers.map((e) => e.toLowerCase()).contains(input);

    setState(() {
      answered = true;
      isCorrect = correct;
      aiExplanation = "Checking your answer...";
    });
    evaluateAnswer(input); // CALL GEMINI
  }

  @override
  void initState() {
    super.initState();

    focusNode.addListener(() {
      setState(() {
        isTyping = focusNode.hasFocus;
      });
    });

    controller.addListener(() {
      setState(() {}); // rebuild so button enable + animations update
    });
  }

  @override
  void dispose() {
    focusNode.dispose();
    controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final step = widget.step;

    return LessonStepScaffold(
      answered: answered,
      isCorrect: isCorrect,
      explanation: aiExplanation,
      hint: isCorrect == false ? step.hint : null,
      buttonText: answered ? "NEXT" : "ANSWER",
      onButtonPressed: !answered
          ? (controller.text.isNotEmpty ? submit : null)
          : widget.onNext,
      child: Column(
        children: [
          Expanded(
              child: SingleChildScrollView(
            padding: const EdgeInsets.fromLTRB(24, 0, 24, 140),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  step.instructions,
                  style: const TextStyle(
                    fontSize: 26,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 10),
                Text(
                  step.title,
                  style: const TextStyle(
                    fontSize: 18,
                    height: 1.5,
                    fontWeight: FontWeight.w600,
                  ),
                ),
                const SizedBox(height: 24),
                Column(
                  children: [
                    Center(
                      child: ClipRRect(
                        borderRadius: BorderRadius.circular(16),
                        child: Image.asset(
                          step.imageAsset,
                          width: 170,
                          fit: BoxFit.cover,
                        ),
                      ),
                    ),
                    const SizedBox(height: 28),
                  ],
                ),
                const SizedBox(height: 28),
                AppTextField(
                  hintText: "Type your answer...",
                  controller: controller,
                  borderRadius: 16,
                  multiline: true,
                  minLines: 5,
                  maxLines: 10,
                  shadowOffset: const Offset(0, 2),
                  focusNode: focusNode,
                ),
              ],
            ),
          )),
        ],
      ),
    );
  }
}
