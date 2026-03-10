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
  final FocusNode focusNode = FocusNode();
  bool isTyping = false;
  final TextEditingController controller = TextEditingController();

  bool? isCorrect;
  bool answered = false;

  void submit() {
    if (answered) return;

    final input = controller.text.trim().toLowerCase();
    final correct =
        widget.step.correctAnswers.map((e) => e.toLowerCase()).contains(input);

    setState(() {
      answered = true;
      isCorrect = correct;
    });

    LessonProgressService.recordAnswer(correct);
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
      explanation: isCorrect == true
          ? step.correctExplanation
          : step.incorrectExplanation,
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
                AnimatedSlide(
                  duration: const Duration(milliseconds: 350),
                  offset: isTyping ? const Offset(0, -0.15) : Offset.zero,
                  child: Transform.scale(
                    scale: isTyping ? 0.7 : 1,
                    alignment: Alignment.topCenter,
                    child: Column(
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
                  ),
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
