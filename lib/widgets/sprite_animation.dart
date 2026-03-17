import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'dart:ui' as ui;

class SpriteSheet extends StatefulWidget {
  final String asset;
  final int columns;
  final int rows;
  final int totalFrames;
  final int fps;
  final double width;
  final double height;
  final bool loop;

  const SpriteSheet({
    super.key,
    required this.asset,
    required this.columns,
    required this.rows,
    required this.totalFrames,
    this.fps = 24,
    this.width = 200,
    this.height = 200,
    this.loop = true,
  });

  @override
  State<SpriteSheet> createState() => _SpriteSheetState();
}

class _SpriteSheetState extends State<SpriteSheet>
    with SingleTickerProviderStateMixin {
  ui.Image? image;
  late AnimationController controller;

  @override
  void initState() {
    super.initState();

    controller = AnimationController(
      vsync: this,
      duration:
          Duration(milliseconds: (1000 * widget.totalFrames ~/ widget.fps)),
    );

    widget.loop ? controller.repeat() : controller.forward();

    _loadImage();
  }

  Future<void> _loadImage() async {
    final data = await rootBundle.load(widget.asset);
    final bytes = data.buffer.asUint8List();
    final codec = await ui.instantiateImageCodec(bytes);
    final frame = await codec.getNextFrame();

    if (!mounted) return;

    setState(() {
      image = frame.image;
    });
  }

  @override
  Widget build(BuildContext context) {
    if (image == null) return const SizedBox();

    return CustomPaint(
      size: Size(widget.width, widget.height),
      painter: _SpritePainter(
        image: image!,
        animation: controller,
        columns: widget.columns,
        rows: widget.rows,
        totalFrames: widget.totalFrames,
      ),
    );
  }

  @override
  void dispose() {
    controller.dispose();
    super.dispose();
  }
}

class _SpritePainter extends CustomPainter {
  final ui.Image image;
  final Animation<double> animation;
  final int columns;
  final int rows;
  final int totalFrames;

  _SpritePainter({
    required this.image,
    required this.animation,
    required this.columns,
    required this.rows,
    required this.totalFrames,
  }) : super(repaint: animation);

  @override
  void paint(Canvas canvas, Size size) {
    int frame = (animation.value * totalFrames).floor() % totalFrames;

    double frameWidth = image.width / columns;
    double frameHeight = image.height / rows;

    int col = frame % columns;
    int row = frame ~/ columns;

    Rect src = Rect.fromLTWH(
      col * frameWidth,
      row * frameHeight,
      frameWidth,
      frameHeight,
    );

    Rect dst = Rect.fromLTWH(0, 0, size.width, size.height);

    canvas.drawImageRect(image, src, dst, Paint());
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}
