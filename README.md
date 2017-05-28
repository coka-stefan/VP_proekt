﻿# Music Player

## 1. Опис на апликацијата
Апликацијата е едноставен music player со можност за зачувување на плејлисти

## 2. Упатство на користење
### 2.1. Додавање на една или повеќе песни
Кога ќе се стартува апликацијата од мени барот може да се додаде една или повеќе песни при што истите веднаш почнуваат да свират

### 2.2. Отварање на веќе зачувана плејлиста
Преку мени барот во Music > Open Playlist може да се отвори претходно зачувана плејлиста во меморија

### 2.3. Креирање на плејлиста
Преку мени барот во View > Playlist се отвара прозорецот за креирање на плејлисти при што се можни 3 различни прегледи, по песна, по артист и по албум.

#### 2.3.1. Додавање на песни во плејлистата
Преку мени барот на прозорецот за плејлиста може да се додаде една или повеќе песни

#### 2.3.2. Зачувување на плејлистата
Преку мени барот на прозорецот за плејлиста може да се зачува плејлистата при што се отвара прозорец за избор на име и локација

#### 2.3.3. Пребарување низ плејлистата
Во прозорецот за плејлистата постои контрола за пребарување на песните кои се веќе додадени во плејлистата

#### 2.3.4. Бришење од плејлистата 
Селектираните песни во плејлистата може да се избришат со кликање на копчето `delete` на тастатурата

## 3. Претставување на проблемот
### 3.1. Податочни структури
Постојат 3 класи кои се користат за организирање на песните.
Податоците за авторите се чуваат во класата `Artist`. За секој автор има листа на албуми кои се објекти од класата `Album` и во секој албум има листа на песни кои се објекти од класата `Song`.

За секоја песна се чува жанрот, насловот, редниот број во албумот, артистот и албумот доколку постојат. Се чува и должината на песната и нејзината локација во меморијата.

За секој албум се чува името, листа на песни, кавер сликата доколку постои во истиот директориум и годината на издавање доколку е достапна.

За секој автор се чува името и листата на албуми.

### 3.1. Playlist
Во објекти од класата `Playlist` се чува и менаџира плејлистата.  