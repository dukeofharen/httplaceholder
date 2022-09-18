---
title: This Week I Learnt: week 37 2019
date: 2019-09-16T21:11:57+02:00
description: This Week I Learnt: week 37 2019
---

## Java: convert LocalDate to string

```java
LocalDate today = LocalDate.now();
System.out.println(DateTimeFormatter.ofPattern("dd-MM-yyyy");
// Outputs "12-09-2019"
```

Source: https://howtodoinjava.com/java/date-time/localdate-format-example/

## Java: Spring Boot integration testing and rolling back the changes

After some fiddling with Spring Boot integration testing this week, I came across a scenario where some of my tests would fail. They only failed when running all tests in one go; running the tests isolated did work.

Apparently, the test data in your H2 database is not cleared after or before running the test. When decorating your test with `@Transactional`, all changes are rolled back after the test has run.

Source: https://stackoverflow.com/questions/49273851/dirtiescontext-is-not-working-with-spring-boot-and-kotlin

## Java: calculating age

This is very simple (if you're using at least JDK 8).

```
public int calculateAge(
  LocalDate birthDate,
  LocalDate currentDate) {
    // validate inputs ...
    return Period.between(birthDate, currentDate).getYears();
}
```

Source: https://www.baeldung.com/java-get-age