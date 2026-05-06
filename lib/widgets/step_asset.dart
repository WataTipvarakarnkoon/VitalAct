import 'package:flutter/material.dart';
import 'package:vitalact/widgets/sprite_animation.dart';

class StepAsset extends StatelessWidget {
  final String asset;
  final double width;
  final double height;
  final int columns;
  final int rows;
  final int totalFrames;
  final int fps;
  final bool loop;

  const StepAsset({
    super.key,
    required this.asset,
    required this.width,
    required this.height,
    this.columns = 50,
    this.rows = 1,
    this.totalFrames = 50,
    this.fps = 25,
    this.loop = true,
  });

  bool get _isSpriteSheet {
    final normalized = asset.replaceAll('\\', '/');
    final segments = normalized.split('/');
    return segments.contains('spritesheet');
  }

  @override
  Widget build(BuildContext context) {
    if (asset.isEmpty) {
      return SizedBox(width: width, height: height);
    }

    if (_isSpriteSheet) {
      return SpriteSheet(
        asset: asset,
        columns: columns,
        rows: rows,
        totalFrames: totalFrames,
        fps: fps,
        width: width,
        height: height,
        loop: loop,
      );
    }

    return Image.asset(
      asset,
      width: width,
      height: height,
      fit: BoxFit.cover,
    );
  }
}
