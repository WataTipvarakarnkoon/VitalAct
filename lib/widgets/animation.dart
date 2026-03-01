import 'dart:async';
import 'dart:ui' as ui;
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

class SpriteExact extends StatefulWidget {
  const SpriteExact({super.key});

  @override
  State<SpriteExact> createState() => _SpriteExactState();
}

class _SpriteExactState extends State<SpriteExact> {
  ui.Image? image;
  Timer? timer;

  // ===== SETTINGS =====
  final int columns = 6;
  final int frameCount = 60;
  final int fps = 24;
  double frameSize = 0;
  int frame = 0;
  // ====================

  @override
  void initState() {
    super.initState();
    _loadImage();
  }

  Future<void> _loadImage() async {
    final data = await rootBundle.load('assets/spritesheet/wave.png');
    final codec = await ui.instantiateImageCodec(data.buffer.asUint8List());
    final fi = await codec.getNextFrame();

    final img = fi.image;
    frameSize = img.width / columns;

    setState(() {
      image = img;
    });

    _startAnimation();
  }

  void _startAnimation() {
    timer = Timer.periodic(
      Duration(milliseconds: (1000 / fps).round()),
      (_) {
        setState(() {
          frame = (frame + 1) % frameCount;
        });
      },
    );
  }

  @override
  void dispose() {
    timer?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (image == null) return const SizedBox();

    return CustomPaint(
      size: const Size(200, 200),
      painter: _Painter(image!, frame, columns, frameSize),
    );
  }
}

class _Painter extends CustomPainter {
  final ui.Image image;
  final int frame;
  final int columns;
  final double frameSize;

  _Painter(this.image, this.frame, this.columns, this.frameSize);

  @override
  void paint(Canvas canvas, Size size) {
    final col = frame % columns;
    final row = frame ~/ columns;

    final src = Rect.fromLTWH(
      col * frameSize,
      row * frameSize,
      frameSize,
      frameSize,
    );

    final dst = Rect.fromLTWH(0, 0, size.width, size.height);

    canvas.drawImageRect(image, src, dst, Paint());
  }

  @override
  bool shouldRepaint(covariant _Painter old) => old.frame != frame;
}
