import 'package:flutter/material.dart';
import 'package:vitalact/theme/app_colors.dart';

class AppTextField extends StatefulWidget {
  final String hintText;
  final bool isPassword;
  final TextEditingController controller;
  final String? Function(String?)? validator;
  final Iterable<String>? autofillHints;

  final double borderRadius;
  final Offset shadowOffset;
  final Color shadowColor;
  final bool shadowEnabled;
  final EdgeInsetsGeometry contentPadding;

  final bool multiline;
  final int minLines;
  final int maxLines;
  final FocusNode? focusNode;

  const AppTextField({
    super.key,
    required this.hintText,
    required this.controller,
    this.validator,
    this.isPassword = false,
    this.autofillHints,
    this.borderRadius = 50,
    this.shadowOffset = const Offset(0, 3),
    this.shadowColor = AppColors.border,
    this.shadowEnabled = true,
    this.contentPadding = const EdgeInsets.symmetric(
      horizontal: 20,
      vertical: 14,
    ),
    this.multiline = false,
    this.minLines = 1,
    this.maxLines = 1,
    this.focusNode,
  });

  @override
  State<AppTextField> createState() => _AppTextFieldState();
}

class _AppTextFieldState extends State<AppTextField> {
  bool _hasError = false;
  bool _obscureText = true;

  @override
  void initState() {
    super.initState();
    _obscureText = widget.isPassword;
  }

  @override
  Widget build(BuildContext context) {
    final radius = BorderRadius.circular(widget.borderRadius);

    return Container(
      decoration: BoxDecoration(
        borderRadius: radius,
        boxShadow: (!widget.shadowEnabled || _hasError)
            ? []
            : [
                BoxShadow(
                  color: widget.shadowColor,
                  offset: widget.shadowOffset,
                ),
              ],
      ),
      child: TextFormField(
        focusNode: widget.focusNode,
        controller: widget.controller,
        obscureText: widget.isPassword ? _obscureText : false,
        autovalidateMode: AutovalidateMode.onUserInteraction,
        autofillHints: widget.autofillHints,
        validator: widget.validator,
        minLines: widget.multiline ? widget.minLines : 1,
        maxLines: widget.multiline ? widget.maxLines : 1,
        keyboardType:
            widget.multiline ? TextInputType.multiline : TextInputType.text,
        onChanged: (value) {
          final error = widget.validator?.call(value);
          setState(() {
            _hasError = error != null;
          });
        },
        decoration: InputDecoration(
          filled: true,
          fillColor: AppColors.surface,
          hintStyle: const TextStyle(
            fontWeight: FontWeight.w700,
            fontSize: 17,
            color: AppColors.textPrimary,
          ),
          hintText: widget.hintText,
          contentPadding: widget.contentPadding,
          suffixIcon: widget.isPassword
              ? IconButton(
                  icon: Icon(
                    _obscureText ? Icons.visibility_off : Icons.visibility,
                  ),
                  onPressed: () {
                    setState(() {
                      _obscureText = !_obscureText;
                    });
                  },
                )
              : null,
          errorBorder: OutlineInputBorder(
            borderRadius: radius,
            borderSide: const BorderSide(
              color: AppColors.error,
              width: 2,
            ),
          ),
          focusedErrorBorder: OutlineInputBorder(
            borderRadius: radius,
            borderSide: const BorderSide(
              color: AppColors.error,
              width: 2,
            ),
          ),
          enabledBorder: OutlineInputBorder(
            borderRadius: radius,
            borderSide: const BorderSide(
              color: AppColors.border,
              width: 2,
            ),
          ),
          focusedBorder: OutlineInputBorder(
            borderRadius: radius,
            borderSide: const BorderSide(
              color: AppColors.border,
              width: 2,
            ),
          ),
        ),
      ),
    );
  }
}
