import 'dart:math';
import 'package:flutter/material.dart';
import 'package:vitalact/l10n/app_localizations.dart';
import 'package:vitalact/theme/app_colors.dart';
import 'package:vitalact/widgets/app_button.dart';

class CprPlacementTestPage extends StatefulWidget {
  const CprPlacementTestPage({super.key});

  @override
  State<CprPlacementTestPage> createState() => _CprPlacementTestPageState();
}

class _CprPlacementTestPageState extends State<CprPlacementTestPage> {
  static const double _correctNX = 0.50;
  static const double _correctNY = 0.50;
  static const double _acceptableRadius = 0.05;

  Offset? _tapNormalized;
  bool? _isCorrect;
  bool _answered = false;
  double _boxW = 1;
  double _boxH = 1;

  void _handleTap(Offset normalized) {
    final dx = normalized.dx - _correctNX;
    final dy = normalized.dy - _correctNY;
    final distance = sqrt(dx * dx + dy * dy);
    setState(() {
      _tapNormalized = normalized;
      _isCorrect = distance <= _acceptableRadius;
      _answered = true;
    });
  }

  void _reset() {
    setState(() {
      _tapNormalized = null;
      _isCorrect = null;
      _answered = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    final bool correct = _isCorrect == true;
    final l10n = AppLocalizations.of(context)!;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        backgroundColor: AppColors.background,
        surfaceTintColor: AppColors.background,
        leading: IconButton(
          padding: const EdgeInsets.only(bottom: 2),
          icon: const Icon(Icons.clear_rounded, color: AppColors.disabled),
          onPressed: () => Navigator.pop(context),
        ),
        title: Text(
          l10n.cprPlacementTitle,
          style: const TextStyle(
            fontSize: 20,
            color: AppColors.disabled,
            fontWeight: FontWeight.w500,
          ),
        ),
        centerTitle: true,
        bottom: const PreferredSize(
          preferredSize: Size.fromHeight(1),
          child: Divider(color: AppColors.disabled, height: 1, thickness: 1),
        ),
      ),
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(24, 20, 24, 4),
              child: Text(
                _answered
                    ? (correct ? l10n.correct : l10n.notQuite)
                    : l10n.cprPlacementInstruction,
                style: TextStyle(
                  fontSize: 26,
                  fontWeight: FontWeight.bold,
                  color: _answered
                      ? (correct ? AppColors.correct : AppColors.incorrect)
                      : AppColors.textPrimaryDark,
                ),
              ),
            ),
            Padding(
              padding: const EdgeInsets.fromLTRB(24, 0, 24, 14),
              child: Text(
                _answered
                    ? (correct
                        ? l10n.cprPlacementCorrectDetail
                        : l10n.cprPlacementIncorrectDetail)
                    : l10n.cprPlacementSubtitle,
                style: TextStyle(
                  fontSize: 15,
                  fontWeight: FontWeight.w600,
                  color: _answered
                      ? (correct
                          ? AppColors.correct.withValues(alpha: 0.8)
                          : AppColors.incorrect.withValues(alpha: 0.8))
                      : AppColors.textPrimary.withValues(alpha: 0.7),
                ),
              ),
            ),

            Expanded(
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 20),
                child: LayoutBuilder(
                  builder: (context, constraints) {
                    _boxW = constraints.maxWidth;
                    _boxH = constraints.maxHeight;
                    return GestureDetector(
                      onTapDown: (details) {
                        if (_answered) return;
                        _handleTap(Offset(
                          (details.localPosition.dx / _boxW).clamp(0.0, 1.0),
                          (details.localPosition.dy / _boxH).clamp(0.0, 1.0),
                        ));
                      },
                      child: ClipRRect(
                        borderRadius: BorderRadius.circular(16),
                        child: Stack(
                          children: [
                            Center(
                              child: Container(
                                decoration: BoxDecoration(
                                    gradient:
                                        RadialGradient(radius: .5, colors: [
                                      Color.fromARGB(204, 83, 10, 10),
                                      Color(0xFFFF4646),
                                    ]),
                                    borderRadius:
                                        BorderRadius.all(Radius.circular(40))),
                                width: 350,
                                height: 420,
                                child: Image.asset(
                                  'assets/images/Idle.png',
                                ),
                              ),
                            ),

                            if (_tapNormalized != null)
                              Align(
                                alignment: Alignment(
                                  _tapNormalized!.dx * 2 - 1,
                                  _tapNormalized!.dy * 2 - 1,
                                ),
                                child: Container(
                                  width: 36,
                                  height: 36,
                                  decoration: BoxDecoration(
                                    shape: BoxShape.circle,
                                    color: (correct
                                            ? AppColors.correct
                                            : AppColors.incorrect)
                                        .withValues(alpha: 0.85),
                                    border: Border.all(
                                      color: Colors.white,
                                      width: 2.5,
                                    ),
                                  ),
                                  child: Icon(
                                    correct ? Icons.check : Icons.close,
                                    color: Colors.white,
                                    size: 20,
                                  ),
                                ),
                              ),

                            // Correct position ring (shown when wrong)
                            if (_answered && !correct)
                              const Align(
                                alignment: Alignment(0, 0),
                                child: _CorrectRing(),
                              ),
                          ],
                        ),
                      ),
                    );
                  },
                ),
              ),
            ),

            const SizedBox(height: 12),

            // Bottom: hint or action buttons
            Padding(
              padding: const EdgeInsets.fromLTRB(20, 0, 20, 16),
              child: SizedBox(
                height: 50,
                child: !_answered
                    ? Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Icon(
                            Icons.touch_app_outlined,
                            color:
                                AppColors.textPrimary.withValues(alpha: 0.45),
                            size: 20,
                          ),
                          const SizedBox(width: 8),
                          Text(
                            l10n.cprPlacementTapHint,
                            style: TextStyle(
                              fontSize: 14,
                              fontWeight: FontWeight.w600,
                              color:
                                  AppColors.textPrimary.withValues(alpha: 0.45),
                            ),
                          ),
                        ],
                      )
                    : Row(
                        children: [
                          if (!correct) ...[
                            Expanded(
                              child: AppButton(
                                variant: ButtonVariant.outlined,
                                width: double.infinity,
                                height: 50,
                                borderRadius: 15,
                                onPressed: _reset,
                                child: Text(l10n.tryAgain),
                              ),
                            ),
                            const SizedBox(width: 12),
                          ],
                          Expanded(
                            child: AppButton(
                              width: double.infinity,
                              height: 50,
                              borderRadius: 15,
                              onPressed: () => Navigator.pop(context),
                              child: Text(l10n.done),
                            ),
                          ),
                        ],
                      ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _CorrectRing extends StatelessWidget {
  const _CorrectRing();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 26,
      height: 26,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        color: AppColors.correct.withValues(alpha: 0.25),
        border: Border.all(color: AppColors.correct, width: 3),
      ),
    );
  }
}
