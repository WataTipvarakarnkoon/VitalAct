import 'package:flutter/material.dart';
import 'package:vitalact/widgets/app_button.dart';
import '../../data/lesson_data.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    final height = MediaQuery.of(context).size.height;

    return Stack(
      children: [
        Positioned(
          top: height * -0.4,
          child: Image.asset(
            'assets/images/Background.png',
            width: width,
          ),
        ),
        Positioned(
          top: height * -0.39,
          left: width * -0.3,
          height: height * 0.5,
          child: Image.asset('assets/images/Ellipse.png'),
        ),
        Positioned(
          top: height * 0.08,
          left: 0,
          right: 0,
          child: Center(
              child: AppButton(
            onPressed: () {},
            width: width * 0.9,
            height: 70,
            borderRadius: 15,
            padding: const EdgeInsetsGeometry.symmetric(horizontal: 0),
            backgroundColor: const Color(0xFFFF4646),
            borderColor: const Color(0xFFFF4646),
            shadowColor: const Color(0xFFCC3838),
            child: const Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  "CONTINUE",
                  style: TextStyle(
                    fontSize: 15,
                    height: 1.2,
                    color: Color(0xB3FFFFFF),
                  ),
                ),
                Text(
                  "Recognition: Life-threatening red flags",
                  style: TextStyle(
                    fontSize: 17,
                    height: 1.1,
                    color: Color(0xFFFFFFFF),
                  ),
                ),
              ],
            ),
          )),
        ),
        Positioned.fill(
          top: height * 0.162,
          child: const Lesson(),
        ),
      ],
    );
  }
}

class Lesson extends StatefulWidget {
  const Lesson({super.key});

  @override
  State<Lesson> createState() => _LessonState();
}

class _LessonState extends State<Lesson> {
  int currentStep = 0;
  int? pressedIndex;

  Future<void> openLesson(int index) async {
    if (index > currentStep) return;

    final result = await Navigator.push(
      context,
      MaterialPageRoute(
        builder: (_) => lessonData[index].page,
      ),
    );

    if (result == true && index == currentStep) {
      setState(() {
        currentStep++;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;

    return SingleChildScrollView(
      physics: const SlowScrollPhysics(),
      child: Column(
        children: [
          const Padding(padding: EdgeInsets.only(top: 50)),
          Container(
            padding: EdgeInsets.only(right: width * .25),
            child: const Text(
              'RECOGNITION',
              style: TextStyle(
                fontSize: 35,
                fontWeight: FontWeight.w800,
                color: Color(0xFFFF4646),
                height: 0.65,
              ),
            ),
          ),
          const SizedBox(height: 10),
          ListView.separated(
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            itemCount: lessonData.length,
            separatorBuilder: (_, __) => const SizedBox(height: 60),
            itemBuilder: (context, index) {
              final lesson = lessonData[index];
              final active = index <= currentStep;
              final isPressed = pressedIndex == index;

              return Center(
                child: FractionallySizedBox(
                  widthFactor: 0.8,
                  child: Material(
                    color: Colors.transparent,
                    child: InkWell(
                      onTap: active ? () => openLesson(index) : null,
                      onHighlightChanged: (pressed) {
                        setState(() {
                          pressedIndex = pressed ? index : null;
                        });
                      },
                      splashColor: Colors.transparent,
                      highlightColor: Colors.transparent,
                      child: AnimatedScale(
                          scale: isPressed ? 0.95 : 1,
                          duration: const Duration(milliseconds: 50),
                          child: SizedBox(
                            width: width * .8,
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                // Lesson Card
                                Stack(
                                  children: [
                                    Image.asset(
                                      active
                                          ? 'assets/images/red_lesson_box.png'
                                          : 'assets/images/gray_lesson_box.png',
                                      fit: BoxFit.contain,
                                    ),
                                    Padding(
                                      padding: const EdgeInsets.all(13),
                                      child: Column(
                                        crossAxisAlignment:
                                            CrossAxisAlignment.start,
                                        children: [
                                          if (active)
                                            Text(
                                              lesson.title,
                                              style: const TextStyle(
                                                fontWeight: FontWeight.w700,
                                                fontSize: 18,
                                                color: Colors.white,
                                              ),
                                            ),
                                          const SizedBox(height: 3),
                                          if (active)
                                            Text(
                                              lesson.description,
                                              style: const TextStyle(
                                                fontSize: 13,
                                                color: Colors.white,
                                              ),
                                            ),
                                        ],
                                      ),
                                    ),
                                  ],
                                ),

                                const SizedBox(height: 12),

                                // Progress Bar
                                if (active)
                                  LinearProgressIndicator(
                                    value: index <= currentStep
                                        ? (index < currentStep ? 1 : 0.3)
                                        : 0,
                                    minHeight: 6,
                                    backgroundColor: Colors.white24,
                                    color: Colors.red,
                                  ),
                              ],
                            ),
                          )),
                    ),
                  ),
                ),
              );
            },
          ),
          const SizedBox(height: 60),
        ],
      ),
    );
  }
}

class SlowScrollPhysics extends BouncingScrollPhysics {
  const SlowScrollPhysics({super.parent});

  @override
  double applyPhysicsToUserOffset(ScrollMetrics position, double offset) {
    return offset * 0.3;
  }
}
