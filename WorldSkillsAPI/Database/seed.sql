-- Тестовые данные для MVP «Наставник по soft skills».
-- Перед запуском примените миграции:
--   dotnet ef database update --project WorldSkillsAPI
-- Запуск из корня проекта:
--   psql -h localhost -p 5432 -U postgres -d "WorldSkillsDB" -f WorldSkillsAPI/Database/seed.sql

BEGIN;

-- Справочники. Обычно эти данные уже добавлены миграциями EF Core.
INSERT INTO "Roles" ("Id", "Name") VALUES
    (1, 'Пользователь'),
    (2, 'Наставник'),
    (3, 'Администратор')
ON CONFLICT ("Id") DO UPDATE SET "Name" = EXCLUDED."Name";

INSERT INTO "ProgressStatuses" ("Id", "Name") VALUES
    (1, 'Не начат'),
    (2, 'В процессе'),
    (3, 'Завершён')
ON CONFLICT ("Id") DO UPDATE SET "Name" = EXCLUDED."Name";

INSERT INTO "AnswerStatuses" ("Id", "Name") VALUES
    (1, 'Не дан'),
    (2, 'На проверке'),
    (3, 'Принят'),
    (4, 'Отклонён')
ON CONFLICT ("Id") DO UPDATE SET "Name" = EXCLUDED."Name";

INSERT INTO "Periodicities" ("Id", "Name") VALUES
    (1, 'Однократно'),
    (2, 'Ежедневно'),
    (3, 'Еженедельно'),
    (4, 'Ежемесячно')
ON CONFLICT ("Id") DO UPDATE SET "Name" = EXCLUDED."Name";

-- Пользователи. Пароли хранятся открыто только потому, что так сейчас устроен API.
INSERT INTO "UserAccounts" ("Login", "Password", "Name", "RoleId")
SELECT 'mentor@skillup.ru', 'mentor123', 'Мария Соколова', 2
WHERE NOT EXISTS (SELECT 1 FROM "UserAccounts" WHERE "Login" = 'mentor@skillup.ru');

INSERT INTO "UserAccounts" ("Login", "Password", "Name", "RoleId")
SELECT 'coach@skillup.ru', 'mentor123', 'Алексей Волков', 2
WHERE NOT EXISTS (SELECT 1 FROM "UserAccounts" WHERE "Login" = 'coach@skillup.ru');

INSERT INTO "UserAccounts" ("Login", "Password", "Name", "RoleId")
SELECT 'anna@student.ru', 'student123', 'Анна Петрова', 1
WHERE NOT EXISTS (SELECT 1 FROM "UserAccounts" WHERE "Login" = 'anna@student.ru');

INSERT INTO "UserAccounts" ("Login", "Password", "Name", "RoleId")
SELECT 'ivan@student.ru', 'student123', 'Иван Кузнецов', 1
WHERE NOT EXISTS (SELECT 1 FROM "UserAccounts" WHERE "Login" = 'ivan@student.ru');

INSERT INTO "UserAccounts" ("Login", "Password", "Name", "RoleId")
SELECT 'olga@student.ru', 'student123', 'Ольга Смирнова', 1
WHERE NOT EXISTS (SELECT 1 FROM "UserAccounts" WHERE "Login" = 'olga@student.ru');

INSERT INTO "UserAccounts" ("Login", "Password", "Name", "RoleId")
SELECT 'admin@skillup.ru', 'admin123', 'Администратор', 3
WHERE NOT EXISTS (SELECT 1 FROM "UserAccounts" WHERE "Login" = 'admin@skillup.ru');

-- Мини-курсы.
INSERT INTO "Courses" ("Name", "Description", "MentorId")
SELECT
    'Управление временем',
    'Практический мини-курс о приоритетах, планировании дня и борьбе с прокрастинацией.',
    u."Id"
FROM "UserAccounts" u
WHERE u."Login" = 'mentor@skillup.ru'
  AND NOT EXISTS (SELECT 1 FROM "Courses" WHERE "Name" = 'Управление временем')
ORDER BY u."Id"
LIMIT 1;

INSERT INTO "Courses" ("Name", "Description", "MentorId")
SELECT
    'Эффективная коммуникация',
    'Активное слушание, понятная обратная связь и уверенное общение в команде.',
    u."Id"
FROM "UserAccounts" u
WHERE u."Login" = 'mentor@skillup.ru'
  AND NOT EXISTS (SELECT 1 FROM "Courses" WHERE "Name" = 'Эффективная коммуникация')
ORDER BY u."Id"
LIMIT 1;

INSERT INTO "Courses" ("Name", "Description", "MentorId")
SELECT
    'Эмоциональный интеллект',
    'Как распознавать эмоции, управлять реакциями и лучше понимать окружающих.',
    u."Id"
FROM "UserAccounts" u
WHERE u."Login" = 'coach@skillup.ru'
  AND NOT EXISTS (SELECT 1 FROM "Courses" WHERE "Name" = 'Эмоциональный интеллект')
ORDER BY u."Id"
LIMIT 1;

INSERT INTO "Courses" ("Name", "Description", "MentorId")
SELECT
    'Командная работа',
    'Роли в команде, ответственность, совместное принятие решений и разрешение конфликтов.',
    u."Id"
FROM "UserAccounts" u
WHERE u."Login" = 'coach@skillup.ru'
  AND NOT EXISTS (SELECT 1 FROM "Courses" WHERE "Name" = 'Командная работа')
ORDER BY u."Id"
LIMIT 1;

-- Уроки курса «Управление временем».
INSERT INTO "Lessons" ("Name", "Content", "VideoUrl", "Position", "CourseId")
SELECT 'Куда уходит время',
       'В течение дня фиксируйте все занятия. В конце дня разделите их на полезные, обязательные и отвлекающие.',
       NULL, 1, c."Id"
FROM "Courses" c
WHERE c."Name" = 'Управление временем'
  AND NOT EXISTS (SELECT 1 FROM "Lessons" WHERE "CourseId" = c."Id" AND "Position" = 1)
ORDER BY c."Id" LIMIT 1;

INSERT INTO "Lessons" ("Name", "Content", "VideoUrl", "Position", "CourseId")
SELECT 'Матрица Эйзенхауэра',
       'Распределите задачи по четырём группам: важные и срочные, важные и несрочные, неважные и срочные, неважные и несрочные.',
       NULL, 2, c."Id"
FROM "Courses" c
WHERE c."Name" = 'Управление временем'
  AND NOT EXISTS (SELECT 1 FROM "Lessons" WHERE "CourseId" = c."Id" AND "Position" = 2)
ORDER BY c."Id" LIMIT 1;

INSERT INTO "Lessons" ("Name", "Content", "VideoUrl", "Position", "CourseId")
SELECT 'Фокус и техника Pomodoro',
       'Работайте 25 минут без отвлечений, затем отдыхайте 5 минут. После четырёх циклов сделайте длинный перерыв.',
       'https://www.youtube.com/watch?v=mNBmG24djoY', 3, c."Id"
FROM "Courses" c
WHERE c."Name" = 'Управление временем'
  AND NOT EXISTS (SELECT 1 FROM "Lessons" WHERE "CourseId" = c."Id" AND "Position" = 3)
ORDER BY c."Id" LIMIT 1;

-- Уроки курса «Эффективная коммуникация».
INSERT INTO "Lessons" ("Name", "Content", "VideoUrl", "Position", "CourseId")
SELECT 'Активное слушание',
       'Слушайте без перебивания, задавайте уточняющие вопросы и кратко пересказывайте услышанное.',
       NULL, 1, c."Id"
FROM "Courses" c
WHERE c."Name" = 'Эффективная коммуникация'
  AND NOT EXISTS (SELECT 1 FROM "Lessons" WHERE "CourseId" = c."Id" AND "Position" = 1)
ORDER BY c."Id" LIMIT 1;

INSERT INTO "Lessons" ("Name", "Content", "VideoUrl", "Position", "CourseId")
SELECT 'Конструктивная обратная связь',
       'Описывайте конкретную ситуацию и поведение, объясняйте его влияние и предлагайте следующий шаг.',
       NULL, 2, c."Id"
FROM "Courses" c
WHERE c."Name" = 'Эффективная коммуникация'
  AND NOT EXISTS (SELECT 1 FROM "Lessons" WHERE "CourseId" = c."Id" AND "Position" = 2)
ORDER BY c."Id" LIMIT 1;

INSERT INTO "Lessons" ("Name", "Content", "VideoUrl", "Position", "CourseId")
SELECT 'Выступление без страха',
       'Сформулируйте одну главную мысль, подкрепите её тремя аргументами и завершите понятным призывом к действию.',
       NULL, 3, c."Id"
FROM "Courses" c
WHERE c."Name" = 'Эффективная коммуникация'
  AND NOT EXISTS (SELECT 1 FROM "Lessons" WHERE "CourseId" = c."Id" AND "Position" = 3)
ORDER BY c."Id" LIMIT 1;

-- Уроки курса «Эмоциональный интеллект».
INSERT INTO "Lessons" ("Name", "Content", "VideoUrl", "Position", "CourseId")
SELECT 'Распознаём эмоции',
       'Несколько раз в день называйте свою эмоцию, её интенсивность и событие, которое её вызвало.',
       NULL, 1, c."Id"
FROM "Courses" c
WHERE c."Name" = 'Эмоциональный интеллект'
  AND NOT EXISTS (SELECT 1 FROM "Lessons" WHERE "CourseId" = c."Id" AND "Position" = 1)
ORDER BY c."Id" LIMIT 1;

INSERT INTO "Lessons" ("Name", "Content", "VideoUrl", "Position", "CourseId")
SELECT 'Пауза перед реакцией',
       'При сильной эмоции сделайте паузу, три спокойных вдоха и только затем выбирайте действие.',
       NULL, 2, c."Id"
FROM "Courses" c
WHERE c."Name" = 'Эмоциональный интеллект'
  AND NOT EXISTS (SELECT 1 FROM "Lessons" WHERE "CourseId" = c."Id" AND "Position" = 2)
ORDER BY c."Id" LIMIT 1;

-- Уроки курса «Командная работа».
INSERT INTO "Lessons" ("Name", "Content", "VideoUrl", "Position", "CourseId")
SELECT 'Роли и сильные стороны',
       'Обсудите, в каких задачах каждый участник команды наиболее эффективен, и распределите ответственность явно.',
       NULL, 1, c."Id"
FROM "Courses" c
WHERE c."Name" = 'Командная работа'
  AND NOT EXISTS (SELECT 1 FROM "Lessons" WHERE "CourseId" = c."Id" AND "Position" = 1)
ORDER BY c."Id" LIMIT 1;

INSERT INTO "Lessons" ("Name", "Content", "VideoUrl", "Position", "CourseId")
SELECT 'Как решать конфликты',
       'Отделяйте человека от проблемы, выясняйте интересы сторон и ищите решение, которое учитывает общую цель.',
       NULL, 2, c."Id"
FROM "Courses" c
WHERE c."Name" = 'Командная работа'
  AND NOT EXISTS (SELECT 1 FROM "Lessons" WHERE "CourseId" = c."Id" AND "Position" = 2)
ORDER BY c."Id" LIMIT 1;

-- Практические задания.
INSERT INTO "Assignments" ("LessonId", "Name", "Description", "MaxScore", "Position")
SELECT l."Id", 'Аудит одного дня',
       'Запишите занятия за один день и найдите минимум два поглотителя времени.', 10, 1
FROM "Lessons" l JOIN "Courses" c ON c."Id" = l."CourseId"
WHERE c."Name" = 'Управление временем' AND l."Position" = 1
  AND NOT EXISTS (SELECT 1 FROM "Assignments" WHERE "LessonId" = l."Id" AND "Position" = 1)
ORDER BY l."Id" LIMIT 1;

INSERT INTO "Assignments" ("LessonId", "Name", "Description", "MaxScore", "Position")
SELECT l."Id", 'План по матрице',
       'Распределите десять актуальных задач по матрице Эйзенхауэра.', 10, 1
FROM "Lessons" l JOIN "Courses" c ON c."Id" = l."CourseId"
WHERE c."Name" = 'Управление временем' AND l."Position" = 2
  AND NOT EXISTS (SELECT 1 FROM "Assignments" WHERE "LessonId" = l."Id" AND "Position" = 1)
ORDER BY l."Id" LIMIT 1;

INSERT INTO "Assignments" ("LessonId", "Name", "Description", "MaxScore", "Position")
SELECT l."Id", 'Тренировка активного слушания',
       'Проведите десятиминутный разговор, используя уточняющие вопросы и перефразирование.', 10, 1
FROM "Lessons" l JOIN "Courses" c ON c."Id" = l."CourseId"
WHERE c."Name" = 'Эффективная коммуникация' AND l."Position" = 1
  AND NOT EXISTS (SELECT 1 FROM "Assignments" WHERE "LessonId" = l."Id" AND "Position" = 1)
ORDER BY l."Id" LIMIT 1;

INSERT INTO "Assignments" ("LessonId", "Name", "Description", "MaxScore", "Position")
SELECT l."Id", 'Дневник эмоций',
       'Зафиксируйте пять эмоциональных ситуаций: эмоцию, причину, реакцию и возможную альтернативу.', 10, 1
FROM "Lessons" l JOIN "Courses" c ON c."Id" = l."CourseId"
WHERE c."Name" = 'Эмоциональный интеллект' AND l."Position" = 1
  AND NOT EXISTS (SELECT 1 FROM "Assignments" WHERE "LessonId" = l."Id" AND "Position" = 1)
ORDER BY l."Id" LIMIT 1;

-- Избранные курсы.
INSERT INTO "FavoriteCourses" ("UserId", "CourseId", "FavoritedAt")
SELECT u."Id", c."Id", CURRENT_TIMESTAMP - INTERVAL '3 days'
FROM "UserAccounts" u CROSS JOIN "Courses" c
WHERE u."Login" = 'anna@student.ru' AND c."Name" = 'Управление временем'
ON CONFLICT ("UserId", "CourseId") DO NOTHING;

INSERT INTO "FavoriteCourses" ("UserId", "CourseId", "FavoritedAt")
SELECT u."Id", c."Id", CURRENT_TIMESTAMP - INTERVAL '1 day'
FROM "UserAccounts" u CROSS JOIN "Courses" c
WHERE u."Login" = 'anna@student.ru' AND c."Name" = 'Эффективная коммуникация'
ON CONFLICT ("UserId", "CourseId") DO NOTHING;

INSERT INTO "FavoriteCourses" ("UserId", "CourseId", "FavoritedAt")
SELECT u."Id", c."Id", CURRENT_TIMESTAMP - INTERVAL '2 days'
FROM "UserAccounts" u CROSS JOIN "Courses" c
WHERE u."Login" = 'ivan@student.ru' AND c."Name" = 'Командная работа'
ON CONFLICT ("UserId", "CourseId") DO NOTHING;

-- Прогресс студентов.
INSERT INTO "LessonProgresses" ("UserId", "LessonId", "StatusId", "StartedAt", "CompletedAt")
SELECT u."Id", l."Id", 3, CURRENT_TIMESTAMP - INTERVAL '5 days', CURRENT_TIMESTAMP - INTERVAL '4 days'
FROM "UserAccounts" u CROSS JOIN "Lessons" l JOIN "Courses" c ON c."Id" = l."CourseId"
WHERE u."Login" = 'anna@student.ru' AND c."Name" = 'Управление временем' AND l."Position" = 1
  AND NOT EXISTS (SELECT 1 FROM "LessonProgresses" p WHERE p."UserId" = u."Id" AND p."LessonId" = l."Id");

INSERT INTO "LessonProgresses" ("UserId", "LessonId", "StatusId", "StartedAt", "CompletedAt")
SELECT u."Id", l."Id", 2, CURRENT_TIMESTAMP - INTERVAL '2 days', NULL
FROM "UserAccounts" u CROSS JOIN "Lessons" l JOIN "Courses" c ON c."Id" = l."CourseId"
WHERE u."Login" = 'anna@student.ru' AND c."Name" = 'Управление временем' AND l."Position" = 2
  AND NOT EXISTS (SELECT 1 FROM "LessonProgresses" p WHERE p."UserId" = u."Id" AND p."LessonId" = l."Id");

INSERT INTO "LessonProgresses" ("UserId", "LessonId", "StatusId", "StartedAt", "CompletedAt")
SELECT u."Id", l."Id", 2, CURRENT_TIMESTAMP - INTERVAL '1 day', NULL
FROM "UserAccounts" u CROSS JOIN "Lessons" l JOIN "Courses" c ON c."Id" = l."CourseId"
WHERE u."Login" = 'ivan@student.ru' AND c."Name" = 'Командная работа' AND l."Position" = 1
  AND NOT EXISTS (SELECT 1 FROM "LessonProgresses" p WHERE p."UserId" = u."Id" AND p."LessonId" = l."Id");

-- Напоминания.
INSERT INTO "Reminders" ("UserId", "CourseId", "Text", "RemindAt", "PeriodicityId", "IsRead", "NextTriggerAt")
SELECT u."Id", c."Id", 'Составить план задач на завтра',
       CURRENT_TIMESTAMP + INTERVAL '2 hours', 2, FALSE, CURRENT_TIMESTAMP + INTERVAL '1 day 2 hours'
FROM "UserAccounts" u CROSS JOIN "Courses" c
WHERE u."Login" = 'anna@student.ru' AND c."Name" = 'Управление временем'
  AND NOT EXISTS (
      SELECT 1 FROM "Reminders" r
      WHERE r."UserId" = u."Id" AND r."Text" = 'Составить план задач на завтра'
  );

INSERT INTO "Reminders" ("UserId", "CourseId", "Text", "RemindAt", "PeriodicityId", "IsRead", "NextTriggerAt")
SELECT u."Id", c."Id", 'Пройти следующий урок курса',
       CURRENT_TIMESTAMP + INTERVAL '1 day', 3, FALSE, CURRENT_TIMESTAMP + INTERVAL '8 days'
FROM "UserAccounts" u CROSS JOIN "Courses" c
WHERE u."Login" = 'ivan@student.ru' AND c."Name" = 'Командная работа'
  AND NOT EXISTS (
      SELECT 1 FROM "Reminders" r
      WHERE r."UserId" = u."Id" AND r."Text" = 'Пройти следующий урок курса'
  );

-- Сообщения в обсуждениях уроков.
INSERT INTO "Messages" ("Content", "CreatedAt", "UserId", "LessonId")
SELECT 'Какие приложения вы используете для учёта времени?', CURRENT_TIMESTAMP - INTERVAL '2 days', u."Id", l."Id"
FROM "UserAccounts" u CROSS JOIN "Lessons" l JOIN "Courses" c ON c."Id" = l."CourseId"
WHERE u."Login" = 'anna@student.ru' AND c."Name" = 'Управление временем' AND l."Position" = 1
  AND NOT EXISTS (
      SELECT 1 FROM "Messages" m
      WHERE m."UserId" = u."Id" AND m."LessonId" = l."Id"
        AND m."Content" = 'Какие приложения вы используете для учёта времени?'
  );

INSERT INTO "Messages" ("Content", "CreatedAt", "UserId", "LessonId")
SELECT 'Начните с обычных заметок: важнее регулярно фиксировать занятия, чем выбрать идеальный инструмент.',
       CURRENT_TIMESTAMP - INTERVAL '1 day 23 hours', u."Id", l."Id"
FROM "UserAccounts" u CROSS JOIN "Lessons" l JOIN "Courses" c ON c."Id" = l."CourseId"
WHERE u."Login" = 'mentor@skillup.ru' AND c."Name" = 'Управление временем' AND l."Position" = 1
  AND NOT EXISTS (
      SELECT 1 FROM "Messages" m
      WHERE m."UserId" = u."Id" AND m."LessonId" = l."Id"
        AND m."Content" LIKE 'Начните с обычных заметок:%'
  );

-- Ответ студента и оценка наставника.
INSERT INTO "Answers" ("AssignmentId", "UserId", "Content", "FileUrl", "SubmittedAt", "StatusId")
SELECT a."Id", u."Id",
       'Главные поглотители времени — уведомления и частое переключение между задачами. Отключила уведомления и выделила два блока фокусной работы.',
       '', CURRENT_TIMESTAMP - INTERVAL '3 days', 3
FROM "Assignments" a
JOIN "Lessons" l ON l."Id" = a."LessonId"
JOIN "Courses" c ON c."Id" = l."CourseId"
CROSS JOIN "UserAccounts" u
WHERE a."Name" = 'Аудит одного дня' AND c."Name" = 'Управление временем'
  AND u."Login" = 'anna@student.ru'
  AND NOT EXISTS (SELECT 1 FROM "Answers" x WHERE x."AssignmentId" = a."Id" AND x."UserId" = u."Id");

INSERT INTO "Grades" ("AnswerId", "Score", "GradedAt", "MentorId")
SELECT a."Id", 9, CURRENT_TIMESTAMP - INTERVAL '2 days', m."Id"
FROM "Answers" a
JOIN "UserAccounts" s ON s."Id" = a."UserId"
CROSS JOIN "UserAccounts" m
WHERE s."Login" = 'anna@student.ru' AND m."Login" = 'mentor@skillup.ru'
  AND NOT EXISTS (SELECT 1 FROM "Grades" g WHERE g."AnswerId" = a."Id");

COMMIT;

-- Тестовые аккаунты:
-- mentor@skillup.ru / mentor123
-- coach@skillup.ru  / mentor123
-- anna@student.ru   / student123
-- ivan@student.ru   / student123
-- olga@student.ru   / student123
-- admin@skillup.ru  / admin123
