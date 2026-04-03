import 'package:vitalact/screens/main/test_unity.dart';
import 'package:flutter/material.dart';
import 'package:vitalact/theme/app_colors.dart';
import 'package:vitalact/widgets/sprite_animation.dart';
import 'package:vitalact/screens/lessons/rapid_response.dart';
import 'package:vitalact/models/lesson_step.dart';
import 'package:vitalact/models/steps/reading_step.dart';
import 'package:vitalact/data/lesson_data.dart';
import 'package:vitalact/screens/lessons/lesson_runner_page.dart';

class PracticePage extends StatefulWidget {
  const PracticePage({super.key});

  @override
  State<PracticePage> createState() => _PracticePageState();
}

class _PracticePageState extends State<PracticePage>
    with SingleTickerProviderStateMixin {
  bool isPressed = false;
  late final TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final height = MediaQuery.of(context).size.height;

    return AnimatedBuilder(
      animation: _tabController.animation!,
      builder: (context, _) {
        final double t = _tabController.animation!.value;
        final Color themeColor =
            Color.lerp(AppColors.primary, AppColors.secondary, t)!;

        return AnimatedContainer(
          duration: const Duration(milliseconds: 150),
          color: themeColor.withValues(alpha: 0.05),
          child: Column(
            children: [
              const SizedBox(height: 40),
              TabBar(
                indicatorWeight: 1,
                indicatorSize: TabBarIndicatorSize.tab,
                controller: _tabController,
                indicatorColor: themeColor,
                labelColor: themeColor,
                unselectedLabelColor: AppColors.textPrimary,
                labelStyle:
                    const TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
                unselectedLabelStyle:
                    const TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
                tabs: const [
                  Tab(text: "Mental Drill"),
                  Tab(text: "Physical Drill"),
                ],
              ),
              Expanded(
                child: TabBarView(
                  controller: _tabController,
                  children: [
                    Center(
                      child: Column(
                        children: [
                          const SizedBox(
                            height: 25,
                          ),
                          Text(
                            "Practice",
                            style: TextStyle(
                              fontSize: 25,
                              fontWeight: FontWeight.w800,
                              color: themeColor,
                            ),
                          ),
                          const Text('Sharpen your emergency skills',
                              style: TextStyle(
                                fontSize: 17,
                                fontWeight: FontWeight.w500,
                                color: AppColors.textPrimary,
                              )),
                          const SizedBox(
                            height: 30,
                          ),
                          GestureDetector(
                            onTapDown: (_) => setState(() => isPressed = true),
                            onTapUp: (_) => setState(() => isPressed = false),
                            onTapCancel: () =>
                                setState(() => isPressed = false),
                            onTap: () {
                              final steps = pickRandomQuestions(lessonData)
                                  .map((q) => q.qs as LessonStep)
                                  .where((step) => step is! ReadingStep)
                                  .toList();

                              Navigator.push(
                                context,
                                MaterialPageRoute(
                                  builder: (context) => LessonRunnerPage(
                                    title: "Rapid Response",
                                    steps: steps,
                                  ),
                                ),
                              );
                            },
                            child: AnimatedScale(
                              scale: isPressed ? 0.8 : 0.9,
                              duration: const Duration(milliseconds: 100),
                              child: Stack(
                                children: [
                                  Align(
                                    alignment: const Alignment(.1, 0),
                                    child: Image.asset(
                                      'assets/images/rapid_response.png',
                                      height: 320,
                                    ),
                                  ),
                                  Positioned(
                                    top: height * .06,
                                    left: 0,
                                    right: 0,
                                    child: const Align(
                                      alignment: Alignment.center,
                                      child: SpriteSheet(
                                        asset: 'assets/spritesheet/NVSA.png',
                                        columns: 50,
                                        rows: 1,
                                        totalFrames: 50,
                                        fps: 30,
                                        height: 140,
                                        width: 140,
                                      ),
                                    ),
                                  )
                                ],
                              ),
                            ),
                          )
                        ],
                      ),
                    ),
                    Center(
                      child: Column(
                        children: [
                          const SizedBox(
                            height: 25,
                          ),
                          Text(
                            "Practice",
                            style: TextStyle(
                              fontSize: 25,
                              fontWeight: FontWeight.w800,
                              color: themeColor,
                            ),
                          ),
                          const Text('Sharpen your emergency skills',
                              style: TextStyle(
                                fontSize: 16,
                                fontWeight: FontWeight.w500,
                                color: AppColors.textPrimary,
                              )),
                          const SizedBox(
                            height: 30,
                          ),
                          GestureDetector(
                            onTapDown: (_) => setState(() => isPressed = true),
                            onTapUp: (_) => setState(() => isPressed = false),
                            onTapCancel: () =>
                                setState(() => isPressed = false),
                            onTap: () {
                              Navigator.push(
                                context,
                                MaterialPageRoute(
                                  builder: (context) => const TestUnity(),
                                ),
                              );
                            },
                            child: AnimatedScale(
                              scale: isPressed ? 0.8 : 0.9,
                              duration: const Duration(milliseconds: 100),
                              child: Stack(
                                children: [
                                  Image.asset(
                                      'assets/images/Emergency Simulator.png'),
                                  const Center(
                                      child: SpriteSheet(
                                    asset: 'assets/spritesheet/CPR.png',
                                    columns: 20,
                                    rows: 1,
                                    totalFrames: 20,
                                    fps: 30,
                                    height: 150,
                                    width: 150,
                                  ))
                                ],
                              ),
                            ),
                          )
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        );
      },
    );
  }
}
