# BÁO CÁO DỰ ÁN 1 — PRO124

## Game Bóng tối huyền bí (Dark Mystery)

**Giảng viên hướng dẫn:** Phí Đức Chính
**Môn học:** Lập trình game — Dự án 1 (PRO124)
**Nhóm thực hiện:** 01

| STT | Họ và tên | Mã sinh viên |
| --- | --- | --- |
| 1 | Dương Hữu Quốc | PH31982 |
| 2 | Hoàng Đặng Anh Tú | PH30909 |
| 3 | Đào Thuý Hoà | PH31026 |
| 4 | Phương Văn Định | PH31545 |
| 5 | Hoàng Thanh Phú | PH31051 |

Hà Nội – 2026

---

# CHƯƠNG 2: TỔNG QUAN DỰ ÁN

## 2.1 Lý do chọn đề tài

**"Bóng tối huyền bí"** là một tựa game thuộc thể loại **card battle roguelike** (chiến đấu thẻ bài kết hợp hành trình), lấy cảm hứng từ những dòng game như *Slay the Spire*. Trò chơi hướng tới đối tượng học sinh từ 8–15 tuổi, mang phong cách đồ hoạ 2D Pixel.

Người chơi vào vai một anh hùng dấn thân vào thế giới bị bóng tối bao phủ, chiến đấu với quái vật bằng bộ bài, thu thập relic (vật phẩm tăng sức mạnh), mở rương kho báu, nghỉ ngơi hồi máu và cuối cùng đối đầu với trùm ở mỗi hòn đảo. Trò chơi có **4 hòn đảo** với độ khó tăng dần, mỗi đảo có một con trùm riêng, và đảo cuối cùng là trùm cuối — vượt qua trùm cuối sẽ hiện màn hình **"You Win"** và người chơi được quay về đảo 1 để chơi tiếp với bộ bài đã thu thập được giữ nguyên.

Nhóm lựa chọn đề tài này vì:
- Game thẻ bài ít phụ thuộc vào kỹ năng điều khiển thời gian thực, phù hợp với mức độ sinh viên.
- Hệ thống bài + relic cho phép xây dựng nhiều tính năng phong phú, dễ phân chia công việc cho 5 thành viên.
- Phát triển được nhiều kỹ năng quan trọng: lập trình C# trên Unity, thiết kế hệ thống, lưu trữ dữ liệu, đồ hoạ và âm thanh.

## 2.2 Mục tiêu đề tài

### 2.2.1 Nghiên cứu và áp dụng kiến thức
- Vận dụng kiến thức lập trình hướng đối tượng C# và môi trường phát triển game Unity Engine.
- Áp dụng các mẫu thiết kế (Singleton, Object Pool, ScriptableObject) vào dự án thực tế.
- Tìm hiểu cách tích hợp dịch vụ bên ngoài (Firebase) vào game Unity.

### 2.2.2 Xây dựng nguyên mẫu game
- Xây dựng bản chơi thử (playable prototype) hoàn chỉnh một vòng chơi: đăng nhập → chọn đảo → đi map → đánh quái/boss → nhận thưởng → thắng game.
- Hỗ trợ lưu tiến trình lên cloud (Firebase) để người chơi có thể tiếp tục trên thiết bị khác.

## 2.3 Công nghệ sử dụng

### 2.3.1 Các công cụ
- **Unity 2022 (URP 2D)** — công cụ phát triển game chính.
- **Visual Studio Code / Visual Studio** — soạn thảo và gỡ lỗi mã C#.
- **Git + GitHub** — quản lý phiên bản mã nguồn, làm việc nhóm (nhánh `main`, `test9`, `test10`…).
- **Git LFS** — quản lý các file plugin kích thước lớn (Firebase SDK).
- **Google Sheets / Trello** — phân chia công việc và theo dõi tiến độ.

### 2.3.2 Các công nghệ
- **Unity Engine** (ngôn ngữ C#) — xây dựng toàn bộ game.
- **Firebase Auth + Firestore** — đăng ký/đăng nhập và lưu tiến trình trò chơi lên cloud.
- **TextMesh Pro** — hiển thị chữ trong game.
- **External Dependency Manager (EDM4U)** — quản lý plugin Android/iOS của Firebase.
- **Hệ thống ScriptableObject** — định nghĩa bài, relic, quái vật dữ liệu.

## 2.4 Phạm vi đề tài

### 2.4.1 Bao gồm (In Scope)
- Hệ thống đăng nhập/đăng ký (đăng nhập cục bộ bằng file + đồng bộ tài khoản Firebase).
- Bốn hòn đảo (MapLevel 1–4) với các node: Trận đánh, Trùm, Quái nhỏ (Mini Boss), Rương kho báu, Cửa hàng, Nghỉ ngơi.
- Hệ thống bài: tấn công, phòng thủ, hồi máu, rút bài, hiệu ứng trạng thái (Độc, Cháy, Yếu, Dễ tổn thương…), bài nguyền.
- Hệ thống quái vật và trí tuệ nhân tạo (không lặp cùng một hành động quá 2 lượt liên tiếp).
- Hệ thống relic (18 loại), cửa hàng (mua bài, relic, nâng cấp bài).
- Lưu tiến trình lên Firebase Firestore và khôi phục khi đăng nhập lại.
- Màn hình chiến thắng, chỉnh âm lượng (Music/SFX).

### 2.4.2 Không bao gồm (Out of Scope)
- Nhiều người chơi (PvP / co-op online).
- Có thêm đảo/nội dung mới ngoài 4 đảo.
- Hỗ trợ đầy đủ chế độ chơi theo mùa, nhiệm vụ thành tựu.

## 2.5 Phương pháp thực hiện
1. **Nghiên cứu lý thuyết** — tìm hiểu Unity, mẫu thiết kế, Firebase.
2. **Phân tích và thiết kế hệ thống** — vẽ sơ đồ usecase, lớp, thiết kế cơ chế trò chơi.
3. **Phát triển và lập trình** — làm lần lượt từng hệ thống: bài → trận đánh → map → quái/AI → relic → shop → đăng nhập/cloud.
4. **Thực hiện hoá** — chơi thử, chỉnh cân bằng, sửa lỗi.
5. **Kiểm thử và đánh giá** — kiểm thử chức năng từng màn hình, sửa lỗi compile/logic.
6. **Viết báo cáo** — tổng hợp tài liệu.

---

# CHƯƠNG 3: KHẢO SÁT

## 3.1 Thống kê kết quả khảo sát

### 3.1.1 Khảo sát
Khảo sát nhanh với nhóm đối tượng học sinh (8–15 tuổi) về các chức năng mong muốn trong game:
- Trải nghiệm và gameplay: chơi dễ hiểu, mỗi màn đấu ngắn.
- Chế độ chơi: chơi đơn, có tiến trình lưu lại.
- Nhân vật và hệ thống nâng cấp: nâng cấp thẻ bài, thu thập vật phẩm.
- Animation: quái vật và nhân vật có hoạt cảnh khi tấn công.
- Nhiệm vụ, thành tựu: màn hình chiến thắng sau trùm cuối.
- Đồ hoạ, giao diện: giao diện rõ ràng, thân thiện với di động.
- Âm thanh: nhạc nền theo vùng bản đồ, hiệu ứng tiếng đánh.
- Lịch sử chơi / Thành tích: lưu tiến trình lên cloud.

### 3.1.2 Tóm tắt yêu cầu khách hàng
- Game chạy mượt trên điện thoại, thao tác chủ yếu là chạm/kéo thẻ bài.
- Người chơi xây dựng bộ bài qua các trận thắng, mua ở cửa hàng, nhận từ rương.
- Có hệ thống đăng nhập và lưu tiến trình để không mất dữ liệu khi thoát game.

### 3.1.3 Đánh giá tính khả thi
- Nhóm có 5 thành viên, kiến thức C# và Unity cơ bản; thời gian 1 kỳ học.
- Rủi ro: thời gian làm đồ hoạ lâu → giảm thiểu bằng cách dùng asset 2D có sẵn + chỉnh sửa màu, vẽ pixel đơn giản.
- Rủi ro: lỗi tích hợp Firebase (khởi tạo SDK) → dành thời gian kiểm thử sớm, có phương án đăng nhập cục bộ dự phòng.

### 3.1.4 Phân tích yêu cầu và danh mục công việc
- Giao diện người dùng: màn đăng nhập/đăng ký, menu chính, bản đồ, trận đánh, cửa hàng, codex thẻ bài, chỉnh âm lượng.
- Tính năng gameplay: đánh bài, trạng thái, AI quái, boss, relic, rương, nghỉ ngơi.
- Dữ liệu và đồng bộ: lưu cục bộ + Firebase Firestore.

## 3.2 Khảo sát thị trường game

### 3.2.1 Xu hướng
Thể loại roguelike deck-builder (xây bộ bài) đang rất phổ biến trên mobile và PC nhờ tính tái chơi cao, mỗi lần chơi là một bộ bài khác nhau.

### 3.2.2 Tiềm năng
- Phù hợp với thiết bị cấu hình thấp, chơi offline chủ yếu.
- Cơ chế đơn giản nhưng có chiều sâu chiến thuật, dễ giới thiệu tới học sinh.

### 3.2.3 Sản phẩm tương tự
- **Slay the Spire**: tham khảo cơ chế bộ bài, map tuyến tính, relic. Điểm mạnh: cân bằng tốt. Điểm yếu: đồ hoạ tối giản.
- **Dungeon of the Endless**: tham khảo cách chia đảo/vùng. Điểm yếu: độ khó cao với người mới.
- **Monster Slayers**: tham khảo cách rút bài mỗi lượt. Điểm yếu: nội dung hạn chế.
- **Cây trồng / game idle khác**: tham khảo giao diện đơn giản thân thiện trẻ em.

## 3.3 Lập kế hoạch dự án

| TT | Công việc | Bắt đầu | Kết thúc | Thực hiện | Trạng thái |
| --- | --- | --- | --- | --- | --- |
| 1 | Phân tích yêu cầu khách hàng | | | | |
| 1.1 | Vẽ sơ đồ tổng quan hệ thống | 06/2026 | 06/2026 | Cả nhóm | Hoàn thành |
| 1.2 | Vẽ sơ đồ use case | 06/2026 | 06/2026 | Cả nhóm | Hoàn thành |
| 1.3 | Xây dựng bản đặc tả yêu cầu của khách hàng | 06/2026 | 06/2026 | Cả nhóm | Hoàn thành |
| 1.4 | Mô tả nghiệp vụ | 06/2026 | 06/2026 | Cả nhóm | Hoàn thành |
| 2 | Thiết kế hệ thống | | | | |
| 2.1 | Thiết kế hình ảnh bản đồ, cây cảnh ... cho game | 06/2026 | 07/2026 | Đào Thuý Hoà | Hoàn thành |
| 2.2 | Giao diện trận đấu (thẻ bài, HUD, năng lượng) | 06/2026 | 07/2026 | Hoàng Đặng Anh Tú | Hoàn thành |
| 2.3 | Hình ảnh của nhân vật, animation cho nhân vật | 06/2026 | 07/2026 | Dương Hữu Quốc | Hoàn thành |
| 2.4 | Hình ảnh của quái vật, animation cho quái vật | 06/2026 | 07/2026 | Hoàng Thanh Phú | Hoàn thành |
| 2.5 | Thiết kế hệ thống bản đồ 4 đảo và nhiệm vụ | 06/2026 | 07/2026 | Phương Văn Định | Hoàn thành |
| 2.6 | Thiết kế, chọn âm thanh (nhạc nền, hiệu ứng) | 06/2026 | 07/2026 | Đào Thuý Hoà | Hoàn thành |
| 3 | Thực hiện dự án | | | | |
| 3.1 | Hệ thống bài và bộ bài (Deck) | 07/2026 | 07/2026 | Hoàng Đặng Anh Tú | Hoàn thành |
| 3.2 | Hệ thống trận đánh (lượt, năng lượng, trạng thái) | 07/2026 | 07/2026 | Dương Hữu Quốc | Hoàn thành |
| 3.3 | Hệ thống quái vật và trí tuệ nhân tạo (AI) | 07/2026 | 07/2026 | Hoàng Thanh Phú | Hoàn thành |
| 3.4 | Hệ thống map 4 đảo và các node | 07/2026 | 07/2026 | Phương Văn Định | Hoàn thành |
| 3.5 | Hệ thống relic và rương kho báu | 07/2026 | 07/2026 | Đào Thuý Hoà | Hoàn thành |
| 3.6 | Cửa hàng trong game | 07/2026 | 07/2026 | Phương Văn Định | Hoàn thành |
| 3.7 | Đăng nhập / đăng ký | 07/2026 | 07/2026 | Dương Hữu Quốc | Hoàn thành |
| 3.8 | Lưu tiến trình lên Firebase (Cloud Save) | 07/2026 | 07/2026 | Dương Hữu Quốc | Hoàn thành |
| 3.9 | Màn hình chiến thắng (You Win) giữ bộ bài | 07/2026 | 07/2026 | Hoàng Đặng Anh Tú | Hoàn thành |
| 3.10 | Chỉnh âm lượng (nút Settings) | 07/2026 | 07/2026 | Hoàng Đặng Anh Tú | Hoàn thành |
| 3.11 | Cân bằng game (số liệu, độ khó từng đảo) | 07/2026 | 07/2026 | Cả nhóm | Hoàn thành |
| 3.12 | Codex thẻ bài, index quái vật | 07/2026 | 08/2026 | Đào Thuý Hoà | Hoàn thành |
| 3.13 | Tích hợp và chỉnh sửa lỗi phát sinh | 07/2026 | 08/2026 | Cả nhóm | Hoàn thành |
| 4 | Kiểm thử | | | | |
| 4.1 | Xây dựng kịch bản kiểm thử | 08/2026 | 08/2026 | Cả nhóm | Hoàn thành |
| 4.2 | Thực hiện kiểm thử (từng chức năng) | 08/2026 | 08/2026 | Cả nhóm | Hoàn thành |
| 4.3 | Lập báo cáo kiểm thử | 08/2026 | 08/2026 | Cả nhóm | Hoàn thành |
| 4.4 | Lập trình sửa lỗi | 08/2026 | 08/2026 | Cả nhóm | Hoàn thành |
| 5 | Đóng gói và triển khai | | | | |
| 5.1 | Đóng gói bản demo (build APK, kiểm thử trên máy thật) | 08/2026 | 08/2026 | Dương Hữu Quốc | Hoàn thành |
| 5.2 | Bàn giao, hướng dẫn cài đặt và tổng kết dự án | 08/2026 | 08/2026 | Cả nhóm | Hoàn thành |

---

# CHƯƠNG 4: PHÂN TÍCH THIẾT KẾ HỆ THỐNG

## 4.1 Phân tích Usecase

### 4.1.1 Các tác nhân của hệ thống
- **Người chơi (Player)**: người chơi cuối, tương tác chính với game.
- **Hệ thống (System)**: thực hiện logic trận đấu, quản lý dữ liệu, lưu tiến trình.

### 4.1.2 Danh sách Usecase
1. Đăng ký tài khoản
2. Đăng nhập (cục bộ + đồng bộ Firebase)
3. Bắt đầu ván chơi mới / tiếp tục ván đã lưu
4. Chọn đảo và đi bản đồ (chọn node)
5. Đánh bài trong trận
6. Thắng trận nhận thưởng (vàng + chọn bài)
7. Mở rương / nhận relic
8. Mua sắm ở cửa hàng / nâng cấp bài
9. Nghỉ ngơi hồi máu / nâng cấp bài
10. Chỉnh âm lượng
11. Thắng trùm cuối → màn hình You Win
12. Lưu tiến trình lên cloud

### 4.1.3 Sơ đồ UseCase tổng quát
- **Người chơi** nối tới các usecase: Đăng ký, Đăng nhập, Chọn đảo, Đi bản đồ, Đánh bài, Mua sắm, Nghỉ ngơi, Chỉnh âm lượng.
- **Hệ thống** nối tới: Quản lý trận đấu, Quản lý bài/relic/quái, Lưu tiến trình cloud.
- Mối quan hệ:
  - `<<include>>`: "Đánh bài" → "Quản lý năng lượng", "Kiểm tra trạng thái hiệu ứng".
  - `<<extend>>`: "Mở rương" → "Trận gặp Mimic (quái giả rương)" (chỉ khi rơi vào trường hợp 45% là Mimic).

## 4.2 Sơ đồ Usecase phân rã

### 4.2.1 Phân rã chức năng Đăng nhập
| Mô tả | Cho phép người chơi đăng nhập để vào game và khôi phục tiến trình. |
| --- | --- |
| Tác nhân chính | Người chơi |
| Tiền điều kiện | Người chơi đã đăng ký tài khoản (hoặc tạo tài khoản mới) |
| Luồng sự kiện | 1. Người chơi nhập email và mật khẩu<br>2. Hệ thống kiểm tra định dạng email (regex) và độ dài mật khẩu (≥ 6 ký tự)<br>3. Hệ thống kiểm tra tài khoản cục bộ (file users.txt)<br>4. Hệ thống đăng nhập Firebase (SignInWithEmailAndPassword)<br>5. Hệ thống tải tiến trình từ Firestore (nếu có)<br>6. Chuyển sang màn hình Main Menu |
| Luồng phụ | • Sai email/mật khẩu → hiện thông báo "Invalid email or password."<br>• Mất mạng/không kết nối Firebase → vẫn đăng nhập cục bộ được |
| Hậu điều kiện | Người chơi vào Main Menu, tiến trình đã lưu được khôi phục |

### 4.2.2 Phân rã chức năng Trận đánh (Đánh bài)
| Mô tả | Cho phép người chơi dùng các lá bài trên tay để tấn công/phòng thủ quái vật. |
| --- | --- |
| Tác nhân chính | Người chơi, Hệ thống |
| Luồng sự kiện | 1. Hệ thống sinh quái theo bản đồ (RuntimeEnemyLibrary)<br>2. Lượt người chơi: reset năng lượng (4), rút 5 lá<br>3. Người chơi kéo thả lá bài lên quái mục tiêu<br>4. Hệ thống trừ năng lượng, áp hiệu ứng (CardEffectResolver)<br>5. Kết thúc lượt → hệ thống đưa bài trên tay vào discard<br>6. Lượt quái: mỗi quái thực hiện intent theo AI<br>7. Quái chết → nhận vàng + chọn 1 trong 3 thẻ thưởng<br>8. Trùm chết → sang đảo tiếp theo / màn hình You Win |
| Hậu điều kiện | Trận kết thúc với kết quả thắng hoặc thua |

## 4.3 Sơ đồ hoạt động

### Sơ đồ hoạt động luồng Đăng ký
`Bắt đầu → Nhập email/mật khẩu → Kiểm tra định dạng → [Không hợp lệ] → Báo lỗi → Nhập lại`  
`→ [Hợp lệ] → Tạo tài khoản cục bộ → Đăng ký Firebase (CreateUserWithEmailAndPasswordAsync)`  
`→ Đăng ký thành công → Chuyển sang màn Đăng nhập → Kết thúc`

### Sơ đồ hoạt động luồng Trận đánh
`Bắt đầu → Vào trận → Lượt người chơi (rút bài, kéo thả đánh) → Hết năng lượng → Kết thúc lượt`  
`→ Lượt quái (AI chọn hành động) → Kiểm tra quái chết?`  
`→ [Chưa] → Vòng mới`  
`→ [Thắng] → Nhận thưởng → [Trùm?] → Đảo kế / You Win`  
`→ [Thua] → Kết thúc ván`

---

# CHƯƠNG 5: THIẾT KẾ GIAO DIỆN

## 5.1 Thiết kế giao diện Gameplay (HUD)
Trong trận đấu, màn hình hiển thị:
- **Trên cùng**: nút chỉnh âm lượng (Settings) ở góc phải, HP người chơi, năng lượng.
- **Giữa**: khu vực quái vật (hiển thị intent — hành động sắp dùng, block, hiệu ứng trạng thái, thanh máu).
- **Dưới cùng**: tay bài (tối đa 10 lá), kéo thả lên quái mục tiêu.
- **Góc**: nút xem bộ bài (Deck), thanh trạng thái buff/debuff của người chơi.

## 5.2 Thiết kế các Menu chức năng
- **Giao diện đăng nhập**: ô nhập email, ô nhập mật khẩu, nút Đăng nhập, đường dẫn sang Đăng ký, nút vào game.
- **Giao diện đăng ký**: email, mật khẩu, xác nhận mật khẩu, nút Đăng ký, quay lại đăng nhập.
- **Main Menu**: nút Play (tiếp tục/chơi mới), nút Quit (về đăng nhập), nút Settings chỉnh âm lượng.
- **Codex thẻ bài (CardCodex)**: danh mục tất cả các lá bài, bộ lọc All/Attack/Block/Heal/Curse, phân trang 15 lá, hiện bản gốc và bản nâng cấp.
- **Index quái vật (Monster Index)**: danh sách quái vật trong game qua MonsterCatalog.

## 5.3 Giao diện trò chơi
- **Màn 1–4 (MapLevel)**: bản đồ tuyến tính các node (Trận đánh, Trùm, Rương, Cửa hàng, Nghỉ ngơi), hiển thị trạng thái đã đi/đang khóa.
- **World Map**: 4 hòn đảo, đảo tiếp theo mở khóa khi thắng đảo trước.
- **Cửa hàng**: 3 ô mua bài, relic, nút nâng cấp bài (75 vàng), nút làm mới.
- **Rương kho báu**: panel nhận relic ngẫu nhiên; trường hợp gặp Mimic sẽ vào trận đánh.

---

# CHƯƠNG 6: PHÂN TÍCH ĐỐI TƯỢNG

## 6.1 Sơ đồ lớp tổng quát

| Nhóm đối tượng | Mô tả | Ví dụ lớp |
| --- | --- | --- |
| Player | Người chơi, máu, giáp, năng lượng, trạng thái | PlayerHealth, PlayerBlock, PlayerCombat, EnergyManager |
| NPC | Nhân vật trung gian tương tác (chủ cửa hàng, rương…) | ShopManager, ChestRewardManager |
| Enemy | Quái vật, máu, AI, intent, boss | EnemyCombat, EnemyData, EnemyFactory |
| Card | Dữ liệu lá bài, hiệu ứng | CardData, CardEffectResolver, DeckManager |
| Relic | Vật phẩm tăng sức mạnh | RelicManager, RelicData |
| Run | Trạng thái ván chơi, tiến trình | RunSession, MapManager |
| System | Lưu trữ, màn hình, âm thanh | CloudSave, SceneLoader, AudioManager |

## 6.2 Chi tiết các đối tượng chính

### 6.2.1 Chi tiết đối tượng Player

| Thuộc tính | Kiểu dữ liệu | Mô tả |
| --- | --- | --- |
| playerId | string | Mã định danh người chơi (email tài khoản) |
| playerMaxHealth | int | Máu tối đa (80) |
| playerCurrentHealth | int | Máu hiện tại |
| gold | int | Vàng (khởi đầu 100) |
| block | int | Giáp chặn sát thương |
| energy | int | Năng lượng mỗi lượt (4) |
| status | Dictionary<string, int> | Hiệu ứng trạng thái (Poison, Weak, Vulnerable, Strength…) |

| Phương thức (Chức năng) | Mô tả |
| --- | --- |
| TakeDamage() / Heal() | Nhận sát thương / hồi máu |
| AbsorbDamage() | Hấp thụ sát thương bằng giáp (Block) |
| ApplyStatus() | Áp / cập nhật hiệu ứng trạng thái |
| DrawCards() | Rút bài từ bộ bài |
| PlayCard() | Chơi một lá bài, trừ năng lượng |
| EndTurn() | Kết thúc lượt người chơi |

### 6.2.2 Chi tiết đối tượng Enemy

| Thuộc tính | Kiểu | Mô tả |
| --- | --- | --- |
| archetype | EnemyArchetype | Basic, Poison, Lifesteal, Golem, Knight, Assassin, Priest |
| health / block | int | Máu / giáp |
| intent | EnemyIntentType | Hành động sắp thực hiện (tấn công, phòng thủ, buff…) |
| isBoss | bool | Có phải trùm không |

| Phương thức | Mô tả |
| --- | --- |
| DecideAction | AI chọn hành động (không lặp quá 2 lượt cùng loại) |
| ExecuteIntent | Thực hiện hành động đã chọn |
| Split / Summon | Cơ chế quái đặc biệt (Slime tách con, Priest triệu hồi) |

### 6.2.3 Chi tiết đối tượng Card

| Thuộc tính | Kiểu | Mô tả |
| --- | --- | --- |
| cardType | CardType | Attack, Defend, Heal, Draw, Effect, Curse |
| rarity | CardRarity | Common, Rare, Epic |
| target | CardTarget | Self, Enemy, All |
| cost | int | Chi phí năng lượng |
| isUpgraded | bool | Đã nâng cấp chưa |

| Phương thức | Mô tả |
| --- | --- |
| Upgrade() | Nâng cấp lá bài (tăng sát thương/giáp…) |
| Resolve() | Áp hiệu ứng khi chơi |

## 6.3 Cơ sở dữ liệu và quản lý
- **Lưu cục bộ**: PlayerPrefs (vị trí node đã đi `CompletedMapNode`, đảo đã mở khóa `UnlockedIsland`, âm lượng `MusicVolume`/`SfxVolume`), file `users.txt` (tài khoản đăng ký) tại persistentDataPath.
- **Cloud**: Firebase Auth (tài khoản), Firebase Firestore — collection `players`, document = UserId, trường `save` lưu JSON gồm: runActive, mapSceneName, mapLevel, HP max/hiện tại, gold, deck (`name|isUpgraded`), relics, completedNodes, unlockedIsland. Lưu tiến trình tự động khi thắng trùm/quay về map, khôi phục khi đăng nhập lại.

---

# CHƯƠNG 7: THỰC HIỆN DỰ ÁN

## 7.1 Lập trình Cốt lõi

### 7.1.1 Hệ thống điều khiển nhân vật
- Thao tác chính là **kéo thả thẻ bài** (CardDrag, CardHover) lên quái mục tiêu; một số lá bài cần chọn mục tiêu (NeedsTarget) bằng cách drop lên đúng quái.
- Kết thúc lượt bằng nút kết thúc; quái tự động hành động theo lượt (TurnManager).

### 7.1.2 Hệ thống bài và trạng thái
- Bài được định nghĩa bằng ScriptableObject (`CardData`), có thể nâng cấp (`Upgrade`).
- Bộ khởi đầu: 5× Strike, 3× Defend, 1× Bash, 1× Second Wind.
- Hiệu ứng được giải quyết bởi `CardEffectResolver`; hệ thống trạng thái (`CharacterStatus`, `PlayerStatus`, `EnemyStatus`) hỗ trợ Poison, Burn, Weak, Vulnerable, Strength, Stun, Bleed, Regen, Lifesteal, Counter…
- Mỗi 3 lượt hệ thống chèn một lá bài nguyền vào bộ rút (CurseLibrary).

### 7.1.3 Hệ thống trí tuệ nhân tạo
- Quái vật chọn hành động (intent) theo archetype: Basic/Poison/Lifesteal/Golem/Knight/Assassin/Priest.
- **Luật chống lặp**: AI không thực hiện cùng một loại hành động quá 2 lượt liên tiếp (`lastIntentType`, `sameTypeCount`, `IsBlocked`, `WeightIfAvailable`).
- Boss có cơ chế riêng: Golem/Overlord triệu hồi quái, Slime chết tách thành 2 con nhỏ, Priest buff đồng minh.
- Nhiệm vụ các quái được sinh theo bản đồ qua `RuntimeEnemyLibrary` (Slime, Goblin, Bat; Knight, Assassin, Priest; Golem; MiniBoss; Boss theo từng đảo; Mimic ẩn trong rương).

## 7.2 Mô tả cốt truyện

### 7.2.1 Bối cảnh thế giới
Vương quốc bị "bóng tối huyền bí" bao phủ. Bốn hòn đảo nối nhau bằng những cây cầu đá cổ xưa bị ma thuật bảo vệ; kẻ nào muốn vượt qua phải đánh bại thủ lĩnh của từng đảo để giải toả phong ấn.

### 7.2.2 Sự kiện khởi đầu
Người chơi là một lữ khách vô danh trôi dạt vào hòn đảo đầu tiên với bộ bài nhỏ trong tay, chỉ có nguyện vọng tìm đường trở về quê hương sau ánh sáng bên kia bóng tối.

### 7.2.3 Diễn biến chính
Vượt qua từng node trên bản đồ: đánh quái (Slime, Goblin, Bat), mở rương, mua bán ở cửa hàng, nghỉ ngơi hồi máu, đối đầu Mini Boss, tích luỹ relic và thẻ bài mạnh hơn.

### 7.2.4 Diễn biến cao trào
Tại đảo 4, người chơi phải đối mặt với **Boss Golem Overlord** — con trùm cuối có khả năng triệu hồi Golem con. Sau khi hạ gục, màn hình **"You Win!"** xuất hiện.

### 7.2.5 Kết thúc
Người chơi chọn "Continue" để quay về đảo 1 **giữ nguyên bộ bài đã thu thập**, mở ra lối chơi thử thách mới với bộ bài mạnh hơn.

## 7.3 Mô tả luật chơi

### 7.3.1 Cơ chế chiến đấu và thuộc tính
- **Lượt người chơi**: reset năng lượng (mặc định 4, có relic tăng thêm), rút 5 lá, đánh bài tùy ý đến khi hết năng lượng, rồi kết thúc lượt (bài trên tay vào discard).
- **Lượt quái**: mỗi quái thực hiện hành động đã chọn (tấn công / đánh block / buff / gây trạng thái).
- **Sát thương**: sát thương cơ bản + Sức mạnh (Strength); bị Yếu (Weak) giảm còn 75%; bị Dễ tổn thương (Vulnerable) nhận thêm 50%; máu hấp thụ qua giáp (Block) trước.
- **Trạng thái cuối lượt**: độc (Poison) trừ máu cuối mỗi lượt của nạn nhân, giáp bị xóa khi lượt người chơi bắt đầu.
- **Điều kiện thắng**: hạ toàn bộ quái → nhận vàng + chọn 1 trong 3 lá thưởng; hạ trùm đảo → sang đảo tiếp; hạ trùm cuối → You Win.
- **Điều kiện thua**: HP người chơi về 0 → mất ván, quay về menu.

## 7.4 Tổ chức dự án

### 7.4.1 Cấu trúc thư mục
```
Assets/
├── Firebase/            # Đăng nhập/đăng ký + Firebase SDK
│   ├── LoginController.cs, RegisterController.cs, LocalAuthManager.cs
│   └── Plugins/         # Firebase App, Auth, Firestore (Android/iOS)
├── Prefab/              # MapManager, Enemy.prefab
├── Resources/
│   ├── Cards/           # ScriptableObject lá bài
│   ├── Enemies/         # ScriptableObject quái vật
│   ├── Relics/          # ScriptableObject relic
│   ├── Music/           # Nhạc nền (MainMenu, Basic 0-3, Boss 0-3)
│   └── Sounds/          # Slash, Hit, ShieldGain, ShieldTakeDamage
├── Scenes/              # Login, MainMenu, WorldMap, MapLevel1-4, BattleLevel1, Shop
├── script/
│   ├── Battle/          # BattleManager, TurnManager, HandManager, CardEffectResolver
│   ├── Cards/           # CardData, CardDatabase, CardDisplay…
│   ├── Core/            # RunSession, CloudSave, SceneLoader, RuntimeEnemyLibrary…
│   ├── Deck/            # DeckManager
│   ├── Enemy/           # EnemyCombat, EnemyFactory, EnemyIntent…
│   ├── Managers/        # AudioManager, GameManager, InputManager, UIManager
│   ├── Map/             # MapNode, WorldMapManager, ChestRewardManager…
│   ├── Player/          # PlayerHealth, PlayerBlock, PlayerCombat…
│   ├── Relic/           # RelicManager, RelicData…
│   ├── Shop/            # ShopManager
│   ├── Status/          # CharacterStatus, StatusHolderUI…
│   └── UI/              # MainMenuUI, CardCodexUI, GameSettingsUI, RuntimeUi…
└── ScriptableObject/    # Data bài (attack/defend/effect/heal/draw/curse), relic, status
```

### 7.4.2 Phân cấp đối tượng (Hierarchy) chính
- **Mỗi MapLevel scene**: `MapManager` (điều khiển tiến trình), các `MapNode` (battle/shop/chest/rest/boss), `WorldIslandUI`.
- **Trận đánh**: `BattleManager`, `TurnManager`, `HandManager`, quái sinh ra từ `EnemyFactory` theo `RuntimeEnemyLibrary`.
- **Toàn cục**: `GameManager`, `AudioManager`, `CloudSave`, `SceneLoader` (Singleton).

## 7.5 Thư viện sử dụng
- Unity Engine (URP 2D, UnityEngine.UI, TextMesh Pro).
- Firebase SDK (.NET): Firebase.Auth, Firebase.Firestore.
- External Dependency Manager (EDM4U) cho Android/iOS.
- Git LFS cho file plugin lớn.

## 7.6 Mã nguồn
- Repository GitHub: `github.com/VuHongCat/Pro124`
- Quản lý nhánh: `main` (ổn định), `test9`/`test10` (tính năng), pull request merge.

---

# CHƯƠNG 8: KIỂM THỬ

## 8.1 Kiểm thử chức năng
| STT | Chức năng | Kịch bản | Kết quả |
| --- | --- | --- | --- |
| 1 | Đăng ký | Nhập email sai định dạng | Chặn, hiện thông báo |
| 2 | Đăng ký | Nhập mật khẩu < 6 ký tự | Chặn, hiện thông báo |
| 3 | Đăng nhập | Email đúng, sai mật khẩu | Thông báo "Invalid email or password." |
| 4 | Đăng nhập | Đúng email/mật khẩu | Vào Main Menu, tải tiến trình cloud |
| 5 | Đi bản đồ | Chọn node battle | Vào trận đúng quái theo đảo |
| 6 | Đánh bài | Kéo thả bài lên quái | Trừ năng lượng, áp đúng hiệu ứng |
| 7 | AI quái | Quan sát 5 lượt liên tiếp | Không lặp 1 hành động quá 2 lượt |
| 8 | Thắng trận | Hạ hết quái | Nhận vàng + chọn 1/3 lá thưởng |
| 9 | Rương | Mở rương | Nhận relic / hoặc gặp Mimic |
| 10 | Cửa hàng | Mua bài/relic, nâng cấp bài | Trừ vàng đúng, ghi tiến trình |
| 11 | Trùm đảo 1–3 | Hạ trùm | Mở khóa đảo kế, sang map mới |
| 12 | Trùm cuối | Hạ Boss Golem Overlord | Hiện màn hình You Win, giữ deck |
| 13 | Âm lượng | Kéo slider Music/SFX | Nhạc/hiệu ứng đổi theo, lưu PlayerPrefs |
| 14 | Lưu tiến trình | Thoát game, đăng nhập lại | Tiến trình khôi phục từ Firebase |

## 8.2 Kiểm thử hệ thống
- Kiểm thử compile toàn bộ mã nguồn trên Unity Editor, không còn lỗi script.
- Kiểm thử chuyển scene liên tục (Login → MainMenu → WorldMap → Map → Battle → Shop → Map) không bị treo.
- Kiểm thử tình huống mất mạng khi đăng nhập: vẫn đăng nhập cục bộ, không crash.
- Kiểm thử trên Play mode với nhiều scene khởi động để phát hiện lỗi singleton (đã sửa trường hợp "Firebase is not ready" khi Play từ scene Login do backup scene).
- Lỗi gặp phải và đã khắc phục:
  - Trùng nhánh switch trong FirebaseErrorHelper (dừng compile) → gộp case.
  - Lỗi push GitHub "Internal Server Error" → dùng `git push --no-thin`.
  - Quái lặp hành động quá nhiều → thêm luật chống lặp AI.
  - Không có màn kết thúc khi thắng map 4 → thêm màn You Win giữ deck.

---

# KẾT LUẬN

Sau thời gian thực hiện, nhóm đã hoàn thành game **"Bóng tối huyền bí"** với đầy đủ chuỗi trải nghiệm: đăng nhập/đăng ký (cục bộ + Firebase), đi qua 4 hòn đảo, chiến đấu bằng thẻ bài với AI thông minh, thu thập relic và nâng cấp bộ bài, mua sắm, mở rương, và màn hình chiến thắng khi hạ trùm cuối.

Điểm mạnh đạt được:
- Hoàn thiện một vòng chơi roguelike đầy đủ, có tính tái chơi cao.
- Tích hợp thành công Firebase để đăng nhập và lưu tiến trình cloud.
- AI quái vật không lặp hành động, tạo cảm giác công bằng và hợp lý.
- Làm việc nhóm hiệu quả với Git/GitHub (nhiều nhánh, pull request, giải quyết conflict).

Hạn chế:
- Đồ hoạ chưa đồng bộ hoàn toàn (kết hợp asset có sẵn và vẽ tay).
- Chưa có nhiều loại boss/mê cung đa dạng cho mỗi lần chơi.

Định hướng phát triển:
- Thêm nhiều thẻ bài, relic và sự kiện ngẫu nhiên.
- Hỗ trợ nhiều nhân vật với bộ bài khởi đầu khác nhau.
- Thêm bảng xếp hạng và nhiệm vụ thành tựu online.

---

# TÀI LIỆU THAM KHẢO

1. Unity Documentation — docs.unity3d.com (Unity Engine, C# scripting, UI).
2. Firebase Documentation — firebase.google.com/docs (Authentication, Cloud Firestore).
3. Tài liệu môn Lập trình game — PRO124, Trường Cao đẳng FPT Polytechnic.
4. Slay the Spire — Mega Crit Games (tham khảo cơ chế deck-builder).
