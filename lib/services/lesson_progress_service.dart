class LessonProgressService {
  static int correctAnswers = 0;
  static int totalQuestions = 0;

  static int totalScore = 0; // NEW

  static void recordAnswer(bool correct, int score) {
    totalQuestions++;

    if (correct) {
      correctAnswers++;
    }

    totalScore += score;
  }

  static double get averageScore {
    if (totalQuestions == 0) return 0;
    return totalScore / totalQuestions;
  }

  static double get accuracy {
    if (totalQuestions == 0) return 0;
    return correctAnswers / totalQuestions;
  }

  static void reset() {
    correctAnswers = 0;
    totalQuestions = 0;
    totalScore = 0;
  }
}
