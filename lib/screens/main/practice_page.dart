import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:vitalact/l10n/app_localizations.dart';
import 'package:vitalact/models/steps/reading_step.dart';
import 'package:vitalact/screens/lessons/lesson_runner_page.dart';
import 'package:vitalact/screens/main/test_unity.dart';
import 'package:vitalact/screens/practice/cpr_placement_reading_page.dart';
import 'package:vitalact/services/lesson_loader.dart';
import 'package:vitalact/theme/app_colors.dart';
import 'package:vitalact/widgets/sprite_animation.dart';

class PracticePage extends StatefulWidget {
  const PracticePage({super.key});

  @override
  State<PracticePage> createState() => _PracticePageState();
}

class _PracticePageState extends State<PracticePage>
    with SingleTickerProviderStateMixin {
  late final TabController _tabController;

  bool _mentalPressed = false;
  bool _cprPressed = false;
  bool _physicalPressed = false;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    precacheImage(
      const AssetImage('assets/images/mentalDrill.png'),
      context,
    );

    precacheImage(
      const AssetImage('assets/images/Emergency Simulator.png'),
      context,
    );
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Future<void> startMentalDrill(BuildContext context) async {
    final locale = Localizations.localeOf(context).languageCode;
    final drillTitle = AppLocalizations.of(context)!.mentalDrill;
    final modules = await LessonLoader.loadAllModules(locale);

    final lessons = modules.expand((m) => m.lessons).toList();

    final allSteps = lessons
        .expand((lesson) => lesson.steps)
        .where((step) => step is! ReadingStep)
        .toList();

    if (allSteps.isEmpty) return;

    allSteps.shuffle();
    final steps = allSteps.take(10).toList();

    if (!mounted) return;

    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (_) => LessonRunnerPage(
          title: drillTitle,
          steps: steps,
        ),
      ),
    );
  }

  Widget _animatedCard({
    required bool pressed,
    required VoidCallback onTap,
    required ValueChanged<TapDownDetails> onTapDown,
    required VoidCallback onTapUp,
    required Widget child,
  }) {
    return GestureDetector(
      onTapDown: onTapDown,
      onTapUp: (_) => onTapUp(),
      onTapCancel: onTapUp,
      onTap: onTap,
      child: AnimatedScale(
        scale: pressed ? 0.94 : 1,
        duration: const Duration(milliseconds: 100),
        child: child,
      ),
    );
  }

  Widget _sprite(String asset, {double h = 140, double w = 140}) {
    return RepaintBoundary(
      child: SpriteSheet(
        asset: asset,
        columns: 50,
        rows: 1,
        totalFrames: 50,
        fps: 15,
        height: h,
        width: w,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final themeColor = AppColors.primary;
    final height = MediaQuery.of(context).size.height;
    final width = MediaQuery.of(context).size.width;

    return Container(
      color: themeColor.withOpacity(0.05),
      child: Column(
        children: [
          const SizedBox(height: 40),
          TabBar(
            controller: _tabController,
            indicatorColor: themeColor,
            labelColor: themeColor,
            unselectedLabelColor: AppColors.textPrimary,
            tabs: [
              Tab(text: l10n.mentalDrill),
              Tab(text: l10n.physicalDrill),
            ],
          ),
          Expanded(
            child: TabBarView(
              controller: _tabController,
              children: [
                /// Mental Drill
                SingleChildScrollView(
                  child: Column(
                    children: [
                      const SizedBox(height: 25),
                      Text(
                        l10n.practice,
                        style: TextStyle(
                          fontSize: 25,
                          fontWeight: FontWeight.w800,
                          color: themeColor,
                        ),
                      ),
                      Text(l10n.practiceSubtitle),
                      const SizedBox(height: 30),
                      _animatedCard(
                        pressed: _mentalPressed,
                        onTap: () => startMentalDrill(context),
                        onTapDown: (_) => setState(() => _mentalPressed = true),
                        onTapUp: () => setState(() => _mentalPressed = false),
                        child: Stack(
                          children: [
                            Image.asset(
                              'assets/images/mentalDrill.png',
                              height: 320,
                            ),
                            Positioned(
                              top: height * .06,
                              left: 0,
                              right: 0,
                              child: Center(
                                child: _sprite(
                                  'assets/spritesheet/50_frames/NVSA.png',
                                ),
                              ),
                            ),
                            Positioned(
                              top: 240,
                              left: 0,
                              right: 10,
                              child: Center(
                                  child: Text(
                                AppLocalizations.of(context)!.rapidResponse,
                                style: const TextStyle(
                                  fontSize: 23,
                                  fontWeight: FontWeight.w700,
                                  color: AppColors.background,
                                ),
                              )),
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(height: 50),
                      _animatedCard(
                        pressed: _cprPressed,
                        onTap: () => Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (_) => const CprPlacementReadingPage(),
                          ),
                        ),
                        onTapDown: (_) => setState(() => _cprPressed = true),
                        onTapUp: () => setState(() => _cprPressed = false),
                        child: Stack(
                          children: [
                            Image.asset(
                              'assets/images/mentalDrill.png',
                              height: 320,
                            ),
                            Positioned(
                              top: height * .06,
                              left: 0,
                              right: 0,
                              child: Center(
                                child: _sprite(
                                  'assets/spritesheet/50_frames/NVSA.png',
                                ),
                              ),
                            ),
                            Positioned(
                              top: 240,
                              left: 0,
                              right: 10,
                              child: Center(
                                  child: Text(
                                AppLocalizations.of(context)!.handPlacement,
                                style: const TextStyle(
                                  fontSize: 23,
                                  fontWeight: FontWeight.w700,
                                  color: AppColors.background,
                                ),
                              )),
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(height: 50)
                    ],
                  ),
                ),

                /// Physical Drill
                SingleChildScrollView(
                  child: Column(
                    children: [
                      const SizedBox(height: 25),
                      Text(
                        l10n.practice,
                        style: TextStyle(
                          fontSize: 25,
                          fontWeight: FontWeight.w800,
                          color: themeColor,
                        ),
                      ),
                      const SizedBox(height: 30),
                      _animatedCard(
                        pressed: _physicalPressed,
                        onTap: () async {
                          final status = await Permission.camera.request();

                          if (!context.mounted) return;

                          if (status.isGranted) {
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (_) => const TestUnity(),
                              ),
                            );
                          }
                        },
                        onTapDown: (_) =>
                            setState(() => _physicalPressed = true),
                        onTapUp: () => setState(() => _physicalPressed = false),
                        child: Stack(
                          children: [
                            Center(
                              child: Image.asset(
                                'assets/images/Emergency Simulator.png',
                                width: width * .9,
                              ),
                            ),
                            Center(
                              child: RepaintBoundary(
                                child: SpriteSheet(
                                  asset: 'assets/spritesheet/CPR.png',
                                  columns: 20,
                                  rows: 1,
                                  totalFrames: 20,
                                  fps: 12,
                                  height: 150,
                                  width: 150,
                                ),
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
