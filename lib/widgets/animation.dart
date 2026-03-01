import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'dart:ui' as ui;

class SpriteAnimation extends StatefulWidget {
  const SpriteAnimation({super.key});

  @override
  State<SpriteAnimation> createState() => _SpriteAnimationState();
}

class _SpriteAnimationState extends State<SpriteAnimation>
    with SingleTickerProviderStateMixin {
  ui.Image? sprite;
  late AnimationController controller;

  final int totalFrames = 60;
  final int fps = 30;

  static const double userScale = .52;

  @override
  void initState() {
    super.initState();

    controller = AnimationController(
      vsync: this,
      duration: Duration(milliseconds: (1000 * totalFrames ~/ fps)),
    )..repeat();

    loadImage();
  }

  Future<void> loadImage() async {
    final data = await rootBundle.load('assets/spritesheet/wave.png');
    final bytes = data.buffer.asUint8List();
    final codec = await ui.instantiateImageCodec(bytes);
    final frame = await codec.getNextFrame();

    setState(() {
      sprite = frame.image;
    });
  }

  @override
  Widget build(BuildContext context) {
    if (sprite == null) return const SizedBox();

    final screenHeight = MediaQuery.of(context).size.height;

    final frameWidth = sprite!.width / totalFrames;
    final frameHeight = sprite!.height * 0.4;
    final ratio = frameWidth / frameHeight;

    final drawHeight = screenHeight * userScale;
    final drawWidth = drawHeight * ratio;

    return Center(
      child: CustomPaint(
        size: Size(drawWidth, drawHeight),
        painter: SpritePainter(
          image: sprite!,
          animation: controller,
          totalFrames: totalFrames,
        ),
      ),
    );
  }

  @override
  void dispose() {
    controller.dispose();
    super.dispose();
  }
}

class SpritePainter extends CustomPainter {
  final ui.Image image;
  final Animation<double> animation;
  final int totalFrames;

  SpritePainter({
    required this.image,
    required this.animation,
    required this.totalFrames,
  }) : super(repaint: animation);

  @override
  void paint(Canvas canvas, Size size) {
    int frame = (animation.value * totalFrames).floor() % totalFrames;

    double frameWidth = image.width / totalFrames;
    double frameHeight = image.height.toDouble();

    Rect src = Rect.fromLTWH(
      frame * frameWidth,
      0,
      frameWidth,
      frameHeight,
    );

    Rect dst = Rect.fromLTWH(0, 0, size.width, size.height);

    final paint = Paint()
      ..filterQuality = FilterQuality.high
      ..isAntiAlias = true;

    canvas.drawImageRect(image, src, dst, paint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => true;
}
