class LessonProgressService {
  static int correctAnswers = 0;
  static int totalQuestions = 0;

  static void recordAnswer(bool correct) {
    totalQuestions++;

    if (correct) {
      correctAnswers++;
    }
  }

  static void reset() {
    correctAnswers = 0;
    totalQuestions = 0;
  }
}
