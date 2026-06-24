---
name: unity-mcp
description: Use all Unity Editor tools and resources via MCP (CoplayDev unity-mcp). Use when working with Unity or when user mentions Unity, консоль, консоль Unity, ошибки, ошибки компиляции, логи, дебаг, отладка, Debug.Log, компиляция, refresh, recompile. Covers scripts, scenes, assets, GameObjects, menu, console, editor state, animation, camera, graphics, tests.
---

# Unity MCP Tools

Полное описание инструментов и ресурсов Unity MCP (CoplayDev). Сервер в Cursor: `user-unityMCP`. Группы инструментов (core, scene, assets, …) можно отключать в Window > MCP for Unity.

## Оглавление

- [Краткая шпаргалка](#краткая-шпаргалка) — что делать всегда
- [Общие правила](#общие-правила) — схема вызова, пути, пагинация, несколько Unity
- [Ресурсы (чтение)](#ресурсы-чтение) — editor_state, project_info, instances, custom-tools, теги, тесты
- [Инструменты](#инструменты-полный-список) — execute_menu_item, manage_editor, read_console, manage_script, manage_scene, manage_asset, manage_gameobject, manage_shader/graphics/camera/animation, apply_text_edits, script_apply_edits, validate_script
- [Сценарии использования](#сценарии-использования) — правки скриптов, меню, сцены, ассеты, GameObject, Play, теги/слои, компиляция
- [Ошибки и ограничения](#ошибки-и-ограничения)

## Краткая шпаргалка

- **Использовать MCP в каждом запросе:** при любом обращении к проекту (Unity, сцены, ассеты, код) и при любом изменении кода — задействовать инструменты и ресурсы Unity MCP (user-unityMCP): читать состояние через ресурсы, выполнять действия через инструменты, после правок в .cs вызывать Refresh.
- **Сервер:** `user-unityMCP`. Перед вызовом инструмента смотреть схему (ошибка валидации подскажет допустимые `action` и параметры).
- **После правок в .cs:** всегда вызвать **execute_menu_item** с `menu_path: "Assets/Refresh"`; при необходимости проверить **read_console** и **editor_state** (isCompiling).
- **Пути:** только относительно `Assets/`, слэш `/`.
- **Крупные ответы:** пагинация (`page_size`, `cursor`/`next_cursor`); для get_components — сначала `include_properties=false`; для поиска ассетов — `generate_preview=false`.
- **Несколько Unity:** ресурс `mcpforunity://instances` → `set_active_instance` или параметр `unity_instance`. Кастомные инструменты — ресурс `mcpforunity://custom-tools`.

## Общие правила

- **Перед вызовом:** уточнять параметры по схеме (ошибка валидации при неверном вызове показывает допустимые `action` и поля). Использовать `call_mcp_tool` с нужным `server`, `toolName`, `arguments`.
- **Ресурсы** — только чтение состояния. **Инструменты** — выполнение действий.
- **Пути:** всегда относительно `Assets/`, слэш `/`. Не использовать пути вне проекта.
- **Размер ответов:** для больших данных — пагинация (`page_size`, `cursor`/`next_cursor`/`page_number`). Для компонентов сначала запрос с `include_properties=false`, небольшой `page_size` (10–25); с свойствами — только при необходимости, `page_size` 3–10. Для поиска ассетов — `generate_preview=false`, если превью не нужны.
- **Несколько Unity:** ресурс `mcpforunity://instances` даёт список (Name@hash). Затем `set_active_instance` или параметр `unity_instance` в вызове инструмента. Без выбора при нескольких экземплярах сервер может вернуть ошибку.
- **Кастомные инструменты проекта:** сначала проверить ресурс `mcpforunity://custom-tools`.

---

## Ресурсы (чтение)

| Ресурс / URI | Назначение |
|--------------|------------|
| `editor_state` | Состояние редактора: `isCompiling`, и др. Проверять после правок скриптов перед следующими шагами. |
| `project_info` | Информация о проекте (имя, путь, версия Unity и т.д.). |
| `mcpforunity://instances` | Список активных сессий Unity. Нужен при нескольких открытых проектах. |
| `mcpforunity://custom-tools` | Список кастомных инструментов, добавленных в проект. |
| Menu items (ресурс) | Список пунктов меню редактора — для точного `menu_path` в execute_menu_item. |
| `project_tags` | Теги проекта. |
| `project_info` (tests) | Информация о тестах (если доступно). |
| `volumes`, `rendering_stats`, `renderer_features`, `cameras` | Ресурсы по рендеру и камерам (при наличии). |

Консоль читается **инструментом** `read_console`, не ресурсом.

---

## Инструменты (полный список)

### execute_menu_item (manage_menu_item)

Выполнение пункта меню Unity.

- **Параметры:** обязателен `menu_path` или `menuPath` (строка, как в меню Unity).
- Примеры: `Assets/Refresh`, `File/Save Project`, `File/Save`, `Edit/Preferences`, `Tools/EnergyPuzzle/Level From Texture`, `Window/MCP for Unity`.
- **После любых изменений в .cs скриптах** вызывать `menu_path: "Assets/Refresh"`, чтобы запустить перекомпиляцию. Перекомпиляция через manage_editor недоступна.
- Все пункты одной фичи должны использовать один префикс (только `Tools/EnergyPuzzle/...` или только `Tools/Energy Puzzle/...`), иначе в меню появятся два блока.

### manage_editor

Управление и запрос состояния редактора (Play/Pause/Stop, телеметрия, теги, слои, пакеты).

- **action** (точный список смотреть по ошибке валидации): `play`, `pause`, `stop`, `telemetry_status`, `telemetry_ping`, `set_active_tool`, `add_tag`, `remove_tag`, `add_layer`, `remove_layer`, `deploy_package`, `restore_package`. Действия вроде `request_script_reload` или перекомпиляции **нет** — только **execute_menu_item("Assets/Refresh")**.
- При нескольких экземплярах можно передавать `unity_instance` в вызове.

### read_console

Чтение или очистка консоли редактора.

- Параметры: фильтр по типу (Error, Warning, Log), лимиты. После рефреша или правок скриптов проверять ошибки компиляции.

### manage_script

CRUD для C# скриптов в проекте.

- **action:** `create`, `read`, `update`, `delete`.
- **Параметры:** `name` (имя без .cs), `path` (по умолчанию `Assets/`), `contents` (для create/update), `script_type` (например MonoBehaviour), `namespace`.
- После create/update обязательно вызывать **Assets/Refresh** и при необходимости проверять **read_console** и **editor_state.isCompiling**. Для точечных правок предпочтительны **apply_text_edits** или **script_apply_edits** (см. ниже).

### manage_scene

Управление сценами.

- **action:** `load`, `save`, `create`, `get_hierarchy` и др.
- **Параметры:** `name` (сцена без расширения), `path` (по умолчанию `Assets/`), `build_index` (для load/build). Для **get_hierarchy** — пагинация: `page_size` (например 50), `cursor`, следовать `next_cursor` до null.
- В новой сцене должны быть Camera и основной Light (Directional Light).

### manage_asset

Операции с ассетами.

- **action:** `Import`, `Create`, `Modify`, `Delete`, `Duplicate`, `Move`, `Rename`, `Search`, `GetInfo`, `CreateFolder`, `GetComponents`.
- **Параметры:** `path`, `asset_type` (обязателен для Create: например `Material`, `Folder`, `Prefab`), `properties` (словарь для Create/Modify), `destination` (для Move/Duplicate), `search_pattern` (например `*.prefab`), фильтры поиска, `page_size`/`page_number` для Search, `generate_preview` (по возможности false).
- Примеры **properties:** Material: `{"color": [1,0,0,1], "shader": "Standard"}`; Texture: `{"width": 1024, "height": 1024, "format": "RGBA32"}`; PhysicsMaterial: `{"bounciness": 1.0, "staticFriction": 0.5, "dynamicFriction": 0.5}`.
- Префабы создавать через `manage_asset` (action create с типом Prefab или через соответствующий тип).

### manage_gameobject

GameObject’ы и компоненты в сцене.

- **action:** `create`, `modify`, `delete`, `find`, `add_component`, `remove_component`, `set_component_property`, `get_components`, `get_component`.
- **Параметры:** `target` (имя/путь/ид объекта), `search_method` (`by_name`, `by_id`, `by_path`), `name`, `tag`, `parent`, `layer`, `component_properties`, `include_non_public_serialized` (включить private [SerializeField] в ответе). Для **get_components** — сначала `include_properties=false`, небольшой `page_size`; при необходимости запрашивать свойства с маленьким `page_size`.

### manage_shader

Шейдеры: create, read, update, delete. Параметры уточнять по схеме.

### manage_graphics

Объём, постобработка, запекание света, настройки рендера, URP renderer features — много действий (порядка 33). Использовать при работе с рендером/графикой; параметры смотреть по ошибке валидации или документации.

### manage_camera

Камеры, в т.ч. Cinemachine: пресеты, приоритет, шум, блендинг, расширения. Параметры по схеме.

### manage_animation

Анимация: клипы, контроллеры, аниматоры. Параметры по схеме.

### manage_probuilder

Редактирование мешей ProBuilder (если пакет подключён). Параметры по схеме.

### manage_tools

Переключение инструментов редактора в реальном времени. Параметры по схеме.

### apply_text_edits

Точные текстовые правки в файлах: precondition hashes, атомарные батчи правок. Предпочтительно для точечных изменений в коде вместо полной перезаписи через manage_script.

### script_apply_edits

Структурированные правки C#: вставка/замена/удаление методов или классов с заданными границами. Безопаснее для кода, чем грубое перезаписывание файла.

### validate_script

Валидация скрипта (basic или standard) до или после записи. Помогает отловить синтаксис/структуру до компиляции в редакторе. При наличии Roslyn в проекте — более строгая проверка.

---

## Сценарии использования

- **Правки в скриптах (любые):** сохранить файлы → **execute_menu_item("Assets/Refresh")** → при необходимости **read_console** и **editor_state** (isCompiling). Не предлагать следующие шаги до завершения компиляции.
- **Выполнить пункт меню:** **execute_menu_item** с точным `menu_path` (при необходимости взять из ресурса menu items).
- **Загрузить/сохранить сцену:** **manage_scene** (load/save) с `name`, `path`. Новая сцена — create; в сцене должны быть Camera и Directional Light.
- **Иерархия сцены:** **manage_scene** (get_hierarchy) с пагинацией (`page_size`, `cursor`, `next_cursor`).
- **Поиск ассетов:** **manage_asset** (Search) с пагинацией, `generate_preview=false` при ненужности превью.
- **Создать ассет:** **manage_asset** (Create) с `asset_type` и при необходимости `properties`. Папки — CreateFolder.
- **Дублировать/переместить ассет:** **manage_asset** (Duplicate/Move) с `path`, `destination`.
- **Создать префаб:** **manage_asset** (create с типом Prefab) или по документации пакета.
- **Изменить GameObject в сцене:** **manage_gameobject** (create/modify/delete/find, add_component/remove_component/set_component_property). Поиск по имени/пути/ид через `target` и `search_method`.
- **Узнать компоненты объекта:** **manage_gameobject** (get_components) с `include_properties=false` и пагинацией; при необходимости запросить свойства.
- **Play/Pause/Stop:** **manage_editor** (action play/pause/stop).
- **Теги и слои:** **manage_editor** (add_tag, remove_tag, add_layer, remove_layer).
- **Создать/править скрипт:** **manage_script** (create/read/update/delete) или **apply_text_edits** / **script_apply_edits** для точечных правок. После записи — **Assets/Refresh**, при необходимости **validate_script** и **read_console**.
- **Проверить компиляцию после правок:** **read_console** (фильтр Error), **editor_state** (isCompiling). Дождаться окончания компиляции перед следующими действиями.
- **Несколько экземпляров Unity:** прочитать **mcpforunity://instances**, вызвать **set_active_instance** или передать `unity_instance` в следующих вызовах.

---

## Приоритет способов: файлы vs Editor-скрипты

- **У агента есть полный доступ на запись к файлам проекта.** Для дублирования и настройки ассетов (префаб, Addressables) использовать именно его: копирование `.prefab`, правка YAML (имя, `m_Script` guid, поля компонента), создание `.meta` с новым GUID, правка `AssetGroups/*.asset` для Addressables.
- **Дублирование префаба со сменой скрипта:** скопировать файл префаба → в нём заменить guid скрипта и список полей компонента под новый класс → создать `.meta` для копии с новым GUID → добавить запись в группу Addressables с этим GUID и нужным адресом. **Не создавать** Editor-скрипты с пунктом меню для такой одноразовой настройки.
- **Editor-скрипты с меню** — только если пользователь явно просит или операция невыразима правкой файлов (сложная логика в рантайме Unity). Иначе — правка файлов или при необходимости **manage_asset** (Duplicate и т.д.).

## Если что-то «missing» или не работает

- **Сначала доработать текущий подход:** проверить совпадение GUID в `.meta` ассета и в записи группы Addressables; формат записи в `m_SerializeEntries` (поля `m_Address`, `m_SerializedLabels`, `FlaggedDuringContentUpdateRestriction` как у соседних записей). Не переходить на другой способ (например Editor-скрипт), не попробовав исправить файловый вариант.

---

## Ошибки и ограничения

- **Пути:** только относительные от `Assets/`, слэш `/`. Ошибки при неверном пути или при обращении к несуществующему ассету/сцене.
- **Перекомпиляция:** нет отдельного инструмента; только **execute_menu_item("Assets/Refresh")**. manage_editor не выполняет перезагрузку скриптов.
- **Большие ответы:** без пагинации запросы к иерархии, компонентам, поиску ассетов могут вернуть очень большой JSON. Всегда использовать `page_size` и при необходимости `cursor`/`page_number`; для компонентов — сначала без свойств.
- **Меню:** путь должен точно совпадать с пунктом в Unity (регистр, пробелы, слэши). Один префикс на фичу — иначе два блока в меню.
- **Динамические инструменты:** в проекте могут быть кастомные инструменты; проверять **mcpforunity://custom-tools**.

Этот скилл покрывает весь документированный функционал Unity MCP (CoplayDev): все перечисленные инструменты и ресурсы, типовые сценарии и ограничения. Отдельный скилл на каждый инструмент не нужен.
