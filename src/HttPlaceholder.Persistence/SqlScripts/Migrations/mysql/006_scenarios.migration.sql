/*Add scenarios table*/
CREATE TABLE IF NOT EXISTS `scenarios`
(
    `id`        BIGINT(20) NOT NULL AUTO_INCREMENT,
    `scenario`  LONGTEXT   NOT NULL,
    `state`     LONGTEXT   NOT NULL,
    `hit_count` INT        NOT NULL,
    PRIMARY KEY (`id`)
) ENGINE = INNODB
  AUTO_INCREMENT = 1
  DEFAULT CHARSET = UTF8MB4;
