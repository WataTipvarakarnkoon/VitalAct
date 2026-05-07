import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:vitalact/widgets/sprite_animation.dart';

class LessonStepAsset extends StatefulWidget {
  final String asset;
  final int columns;
  final int rows;
  final int totalFrames;
  final int fps;
  final double width;
  final double height;
  final bool loop;
  final BoxFit fit;

  const LessonStepAsset({
    super.key,
    required this.asset,
    required this.columns,
    required this.rows,
    required this.totalFrames,
    this.fps = 25,
    this.width = 172,
    this.height = 172,
    this.loop = true,
    this.fit = BoxFit.cover,
  });

  @override
  State<LessonStepAsset> createState() => _LessonStepAssetState();
}

class _LessonStepAssetState extends State<LessonStepAsset> {
  late Future<bool> _assetExistsFuture;

  @override
  void initState() {
    super.initState();
    _assetExistsFuture = _assetExists();
  }

  @override
  void didUpdateWidget(covariant LessonStepAsset oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.asset != oldWidget.asset) {
      _assetExistsFuture = _assetExists();
    }
  }

  bool get _isSpriteSheet {
    final segments = widget.asset.split(RegExp(r'[\\/]+'));
    return segments.any((segment) => segment.toLowerCase() == 'spritesheet');
  }

  Future<bool> _assetExists() async {
    if (widget.asset.isEmpty) return false;
    try {
      await rootBundle.load(widget.asset);
      return true;
    } catch (_) {
      return false;
    }
  }

  Widget _buildMissingAsset() {
    return Container(
      width: widget.width,
      height: widget.height,
      alignment: Alignment.center,
      decoration: BoxDecoration(
        color: Colors.black12,
        borderRadius: BorderRadius.circular(16),
      ),
      child: const Icon(
        Icons.broken_image,
        size: 36,
        color: Colors.black38,
      ),
    );
  }

  Widget _buildAsset() {
    if (_isSpriteSheet) {
      return SpriteSheet(
        asset: widget.asset,
        columns: widget.columns,
        rows: widget.rows,
        totalFrames: widget.totalFrames,
        fps: widget.fps,
        width: widget.width,
        height: widget.height,
        loop: widget.loop,
      );
    }

    return Image.asset(
      widget.asset,
      width: widget.width,
      height: widget.height,
      fit: widget.fit,
      errorBuilder: (context, error, stackTrace) => _buildMissingAsset(),
    );
  }

  @override
  Widget build(BuildContext context) {
    if (widget.asset.isEmpty) {
      return const SizedBox();
    }

    return FutureBuilder<bool>(
      future: _assetExistsFuture,
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) {
          return SizedBox(width: widget.width, height: widget.height);
        }

        if (snapshot.data == true) {
          return _buildAsset();
        }

        return _buildMissingAsset();
      },
    );
  }
}
