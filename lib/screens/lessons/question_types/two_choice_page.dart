import 'package:flutter/material.dart';
import 'package:vitalact/widgets/lesson/lesson_button.dart';
import '../../../models/steps/two_choice_step.dart';
import '../../../services/lesson_progress_service.dart';
import '../../../widgets/lesson/lesson_feedback_panel.dart';

class TwoChoicePage extends StatefulWidget {
  final TwoChoiceStep step;
  final VoidCallback onNext;

  const TwoChoicePage({
    super.key,
    required this.step,
    required this.onNext,
  });

  @override
  State<TwoChoicePage> createState() => _TwoChoicePageState();
}

class _TwoChoicePageState extends State<TwoChoicePage> {
  bool? isCorrect;
  int? selectedIndex;
  bool answered = false;

  void select(int index) {
    if (answered) return;

    setState(() {
      selectedIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    final step = widget.step;

    return Scaffold(
      backgroundColor: Colors.white,
      body: SafeArea(
        child: Stack(
          children: [
            /// MAIN CONTENT
            Padding(
              padding: const EdgeInsets.only(bottom: 100),
              child: Column(
                children: [
                  Expanded(
                    child: SingleChildScrollView(
                      padding: const EdgeInsets.symmetric(horizontal: 24),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          /// Instructions
                          Text(
                            step.instructions,
                            style: const TextStyle(
                              fontSize: 26,
                              fontWeight: FontWeight.bold,
                            ),
                          ),

                          const SizedBox(height: 10),

                          /// Title
                          Text(
                            step.title,
                            style: const TextStyle(
                              fontSize: 18,
                              height: 1.5,
                              fontWeight: FontWeight.w600,
                            ),
                          ),

                          const SizedBox(height: 24),

                          /// Image
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

                          /// Buttons
                          Row(
                            children:
                                List.generate(step.choices.length, (index) {
                              return Expanded(
                                child: Padding(
                                  padding: EdgeInsets.only(
                                    right: index != step.choices.length - 1
                                        ? 12
                                        : 0,
                                  ),
                                  child: LessonButton(
                                    type: LessonButtonType.option,
                                    text: step.choices[index],
                                    selected: selectedIndex == index,
                                    onPressed:
                                        answered ? null : () => select(index),
                                  ),
                                ),
                              );
                            }),
                          ),

                          const SizedBox(height: 16),

                          /// Disclaimer
                          Text(
                            step.disclaimer,
                            textAlign: TextAlign.center,
                            style: const TextStyle(
                              fontSize: 13,
                              color: Color(0xFF9E9E9E),
                              fontWeight: FontWeight.w500,
                            ),
                          ),

                          const SizedBox(height: 40),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            ),

            /// FEEDBACK PANEL
            LessonFeedbackPanel(
              visible: answered,
              isCorrect: isCorrect ?? false,
              explanation: isCorrect == true
                  ? step.correctExplanation
                  : step.incorrectExplanation,
              hint: isCorrect == false ? step.hint : null,
            ),

            /// NEXT
            Positioned(
              bottom: 0,
              child: LessonButton(
                text: answered ? "NEXT" : "ANSWER",
                onPressed: selectedIndex != null
                    ? () {
                        if (!answered) {
                          setState(() {
                            answered = true;
                            isCorrect =
                                selectedIndex == widget.step.correctIndex;
                          });

                          LessonProgressService.recordAnswer(isCorrect!);
                        } else {
                          widget.onNext();
                        }
                      }
                    : null,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
