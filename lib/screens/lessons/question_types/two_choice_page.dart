import 'package:flutter/material.dart';
import 'package:vitalact/widgets/lesson/lesson_button.dart';
import '../../../models/steps/two_choice_step.dart';

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
  int? selectedIndex;

  void select(int index) {
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
                      children: List.generate(step.choices.length, (index) {
                        return Expanded(
                          child: Padding(
                            padding: EdgeInsets.only(
                              right: index != step.choices.length - 1 ? 12 : 0,
                            ),
                            child: LessonButton(
                              type: LessonButtonType.option,
                              text: step.choices[index],
                              selected: selectedIndex == index,
                              onPressed: () => select(index),
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
            LessonButton(
              text: "ANSWER",
              onPressed: selectedIndex != null ? widget.onNext : null,
            )
          ],
        ),
      ),
    );
  }
}
