import 'package:gradient_borders/gradient_borders.dart';
import 'package:flutter/material.dart';
import 'package:vitalact/theme/app_colors.dart';
import 'package:vitalact/widgets/app_button.dart';
import 'package:vitalact/widgets/sprite_animation.dart';
import 'package:vitalact/data/lesson_data.dart';
import 'package:vitalact/screens/lessons/lesson_runner_page.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    final height = MediaQuery.of(context).size.height;

    return Stack(
      children: [
        Container(
          decoration: const BoxDecoration(
              gradient: LinearGradient(
            colors: [
              AppColors.background,
              AppColors.background,
              AppColors.gradient
            ],
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
          )),
        ),
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
              height: 65,
              borderRadius: 15,
              padding: const EdgeInsetsGeometry.symmetric(horizontal: 0),
              backgroundColor: AppColors.primary,
              borderColor: AppColors.primary,
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    "CONTINUE",
                    style: TextStyle(
                      fontSize: 16,
                      height: 1.2,
                      color: AppColors.background.withValues(alpha: 0.7),
                    ),
                  ),
                  const Text(
                    "Recognition: Life-threatening red flags",
                    style: TextStyle(
                      fontSize: 17,
                      height: 1.1,
                      color: AppColors.background,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
        Positioned.fill(
          top: height * 0.15,
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
        builder: (_) => LessonRunnerPage(
          title: lessonData[index].title,
          steps: lessonData[index].steps,
        ),
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
          const Padding(padding: EdgeInsets.only(top: 40)),
          Container(
            padding: EdgeInsets.only(right: width * .25),
            child: const Text(
              'RECOGNITION',
              style: TextStyle(
                fontSize: 35,
                fontWeight: FontWeight.w800,
                color: AppColors.primary,
                height: 0.65,
              ),
            ),
          ),
          ListView.separated(
            padding: const EdgeInsets.only(top: 10),
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            itemCount: lessonData.length,
            separatorBuilder: (_, __) => const SizedBox(height: 40),
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
                          scale: isPressed ? 0.9 : 1,
                          duration: const Duration(milliseconds: 100),
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
                                    Container(
                                      height: 105,
                                      padding: const EdgeInsets.only(left: 20),
                                      child: Stack(
                                        children: [
                                          if (lesson.spriteAsset != null)
                                            Positioned(
                                              left: 195,
                                              child: SpriteSheet(
                                                asset: lesson.spriteAsset!,
                                                columns: 50,
                                                rows: 1,
                                                totalFrames: 50,
                                                fps: 30,
                                                width: 102,
                                                height: 102,
                                              ),
                                            ),
                                          if (active)
                                            Align(
                                              alignment:
                                                  AlignmentGeometry.centerLeft,
                                              child: SizedBox(
                                                width: 200,
                                                child: Text(
                                                  lesson.title,
                                                  style: const TextStyle(
                                                      fontWeight:
                                                          FontWeight.w700,
                                                      fontSize: 23,
                                                      color:
                                                          AppColors.background,
                                                      height: 1.1,
                                                      shadows: [
                                                        Shadow(
                                                          blurRadius: 5,
                                                          color: Color.fromARGB(
                                                              63, 0, 0, 0),
                                                          offset:
                                                              Offset(0, 2.5),
                                                        )
                                                      ]),
                                                ),
                                              ),
                                            ),
                                          const SizedBox(height: 3),
                                        ],
                                      ),
                                    ),
                                  ],
                                ),
                                const SizedBox(height: 8),

                                // Progress Bar
                                if (active)
                                  Stack(
                                    children: [
                                      Positioned.fill(
                                        child: Container(
                                          decoration: BoxDecoration(
                                              borderRadius:
                                                  const BorderRadius.all(
                                                      Radius.circular(20)),
                                              border: Border.all(
                                                width: 3,
                                                color: AppColors.borderColored,
                                              ),
                                              boxShadow: const [
                                                BoxShadow(
                                                    color: Color.fromARGB(
                                                        44, 128, 0, 0),
                                                    offset: Offset(0, 7),
                                                    blurRadius: .5)
                                              ]),
                                        ),
                                      ),
                                      Container(
                                        margin: const EdgeInsets.all(3.5),
                                        decoration: const BoxDecoration(
                                          borderRadius: BorderRadius.all(
                                              Radius.circular(8)),
                                          border: GradientBoxBorder(
                                            gradient: LinearGradient(
                                              colors: [
                                                AppColors.background,
                                                AppColors.gradient,
                                              ],
                                              begin: Alignment.topLeft,
                                              end: Alignment.bottomRight,
                                            ),
                                            width: 1.5,
                                          ),
                                          boxShadow: [
                                            BoxShadow(
                                              color: Color(0xAAAD3D3D),
                                              offset: Offset(0, 2),
                                              blurRadius: 5,
                                            ),
                                          ],
                                        ),
                                        child: LinearProgressIndicator(
                                          value: index <= currentStep
                                              ? (index < currentStep ? 1 : 0)
                                              : 0,
                                          minHeight: 13,
                                          backgroundColor: Colors.white,
                                          color: AppColors.primary,
                                          borderRadius:
                                              BorderRadius.circular(25),
                                        ),
                                      ),
                                    ],
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
